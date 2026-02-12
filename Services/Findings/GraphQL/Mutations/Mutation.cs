// GraphQL/Mutations/Mutation.cs
using FindingsAPI.Gateway.Services;
using HotChocolate.Authorization;
using FluentValidation;
using FindingsAPI.Gateway.Validators;

namespace FindingsAPI.Gateway.GraphQL.Mutations
{
    [ExtendObjectType("Mutation")]
    public class Mutation
    {
        private readonly IFindingService _findingService;
        private readonly ILogger<Mutation> _logger;

        public Mutation(
            IFindingService findingService,
            ILogger<Mutation> logger)
        {
            _findingService = findingService;
            _logger = logger;
        }

        [GraphQLDescription("Create a new finding")]
        [Authorize(Policy = "Auditor")]
        [Error(typeof(ValidationException))]
        [Error(typeof(ServiceUnavailableException))]
        public async Task<CreateFindingPayload> CreateFinding(
            CreateFindingInput input,
            [Service] IHttpContextAccessor httpContextAccessor)
        {
            _logger.LogInformation("GraphQL Mutation: CreateFinding called by {User}", 
                httpContextAccessor.HttpContext.User.Identity.Name);
            
            // Validate input
            var validator = new CreateFindingInputValidator();
            var validationResult = await validator.ValidateAsync(input);
            
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", 
                    validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            
            try
            {
                var command = new CreateFindingCommand
                {
                    Title = input.Title,
                    Category = input.Category,
                    CompanyId = input.CompanyId,
                    SiteId = input.SiteId,
                    Services = input.Services ?? new List<int>(),
                    Description = input.Description,
                    Severity = input.Severity,
                    CreatedBy = httpContextAccessor.HttpContext.User.Identity.Name
                };
                
                var finding = await _findingService.CreateFindingAsync(command);
                
                // Publish event - TODO: Implement message producer
                // await _messageProducer.PublishAsync(new FindingCreatedEvent
                // {
                //     FindingId = finding.FindingsId,
                //     CompanyId = finding.CompanyId,
                //     CreatedBy = command.CreatedBy,
                //     CreatedAt = DateTime.UtcNow
                // });
                
                return new CreateFindingPayload
                {
                    Finding = finding,
                    Message = "Finding created successfully",
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error creating finding");
                throw new ServiceUnavailableException("Findings service is unavailable");
            }
        }

        [GraphQLDescription("Update an existing finding")]
        [Authorize(Policy = "Auditor")]
        [Error(typeof(FindingNotFoundException))]
        [Error(typeof(ValidationException))]
        public async Task<UpdateFindingPayload> UpdateFinding(
            UpdateFindingInput input,
            [Service] IHttpContextAccessor httpContextAccessor)
        {
            _logger.LogInformation("GraphQL Mutation: UpdateFinding {FindingId} called", 
                input.FindingId);
            
            var command = new UpdateFindingCommand
            {
                FindingId = input.FindingId,
                Status = input.Status,
                Response = input.Response,
                DueDate = input.DueDate,
                UpdatedBy = httpContextAccessor.HttpContext.User.Identity.Name
            };
            
            var updated = await _findingService.UpdateFindingAsync(command);
            
            // Publish event - TODO: Implement message producer
            // await _messageProducer.PublishAsync(new FindingUpdatedEvent
            // {
            //     FindingId = input.FindingId,
            //     OldStatus = updated.OldStatus,
            //     NewStatus = input.Status,
            //     UpdatedBy = command.UpdatedBy,
            //     UpdatedAt = DateTime.UtcNow
            // });
            
            return new UpdateFindingPayload
            {
                Finding = updated,
                Message = "Finding updated successfully",
                Timestamp = DateTime.UtcNow
            };
        }

        [GraphQLDescription("Close a finding")]
        [Authorize(Policy = "Admin")]
        [Error(typeof(FindingNotFoundException))]
        public async Task<CloseFindingPayload> CloseFinding(
            int findingId,
            string closureNotes,
            [Service] IHttpContextAccessor httpContextAccessor)
        {
            var command = new CloseFindingCommand
            {
                FindingId = findingId,
                ClosureNotes = closureNotes,
                ClosedBy = httpContextAccessor.HttpContext.User.Identity.Name
            };
            
            var closedFinding = await _findingService.CloseFindingAsync(command);
            
            return new CloseFindingPayload
            {
                Finding = closedFinding,
                Message = "Finding closed successfully",
                Timestamp = DateTime.UtcNow
            };
        }

        [GraphQLDescription("Bulk update findings status")]
        [Authorize(Policy = "Admin")]
        public async Task<BulkUpdatePayload> BulkUpdateFindingsStatus(
            List<int> findingIds,
            string newStatus,
            string reason,
            [Service] IHttpContextAccessor httpContextAccessor)
        {
            var command = new BulkUpdateStatusCommand
            {
                FindingIds = findingIds,
                NewStatus = newStatus,
                Reason = reason,
                UpdatedBy = httpContextAccessor.HttpContext.User.Identity.Name
            };
            
            var result = await _findingService.BulkUpdateStatusAsync(command);
            
            return new BulkUpdatePayload
            {
                UpdatedCount = result.UpdatedCount,
                FailedCount = result.FailedCount,
                FailedIds = result.FailedIds,
                Message = $"Updated {result.UpdatedCount} findings successfully",
                Timestamp = DateTime.UtcNow
            };
        }
    }
    
    public class CreateFindingInput
    {
        [GraphQLType(typeof(NonNullType<StringType>))]
        public string Title { get; set; }
        
        public string? Description { get; set; }
        
        [GraphQLType(typeof(NonNullType<StringType>))]
        public string Category { get; set; }
        
        [GraphQLType(typeof(NonNullType<IntType>))]
        public int CompanyId { get; set; }
        
        public int? SiteId { get; set; }
        
        public string? Severity { get; set; }
        
        public List<int>? Services { get; set; }
    }
    
    public class CreateFindingPayload
    {
        public Finding? Finding { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}