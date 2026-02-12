using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthProvider.Context;
using AuthProvider.Models.CustomerPortal;
using AuthProvider.DTOs.CustomerPortal;
using AuthProvider.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthProvider.Controllers.CustomerPortal
{
    /// <summary>
    /// Trainings management operations for training programs and courses
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/customerportal/[controller]")]
    [Produces("application/json")]
    [Tags("Customer Portal - Trainings")]
    public class TrainingsController : ControllerBase
    {
        private readonly CustomerPortalContext _context;

        /// <summary>
        /// Initializes a new instance of the TrainingsController
        /// </summary>
        /// <param name="context">Customer Portal database context</param>
        public TrainingsController(CustomerPortalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all trainings with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for filtering trainings by title or description</param>
        /// <param name="isActive">Filter by active status</param>
        /// <param name="trainingType">Filter by training type</param>
        /// <returns>Paginated list of trainings</returns>
        /// <response code="200">Returns paginated list of trainings</response>
        /// <response code="400">Invalid pagination parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponseDto<TrainingResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetTrainings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] TrainingType? trainingType = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(ApiResponse.CreateError("Invalid pagination parameters"));

            var query = _context.Trainings
                .Include(t => t.UserTrainings)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(t => t.Title.Contains(searchTerm) || (t.Description != null && t.Description.Contains(searchTerm)));

            if (isActive.HasValue)
                query = query.Where(t => t.IsActive == isActive.Value);

            if (trainingType.HasValue)
                query = query.Where(t => t.TrainingType == trainingType.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(t => t.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TrainingResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Duration = t.Duration,
                    TrainingType = t.TrainingType,
                    MaxParticipants = t.MaxParticipants,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    EnrolledUsersCount = t.UserTrainings.Count()
                })
                .ToListAsync();

            var response = new PagedResponseDto<TrainingResponseDto> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
            return Ok(ApiResponse<PagedResponseDto<TrainingResponseDto>>.CreateSuccess(response, "Trainings retrieved successfully"));
        }

        /// <summary>
        /// Get a specific training by ID
        /// </summary>
        /// <param name="id">Training unique identifier</param>
        /// <returns>Training details</returns>
        /// <response code="200">Returns training details</response>
        /// <response code="404">Training not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TrainingResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetTraining([Required] int id)
        {
            var training = await _context.Trainings
                .Include(t => t.UserTrainings)
                .Where(t => t.Id == id)
                .Select(t => new TrainingResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Duration = t.Duration,
                    TrainingType = t.TrainingType,
                    MaxParticipants = t.MaxParticipants,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    EnrolledUsersCount = t.UserTrainings.Count()
                })
                .FirstOrDefaultAsync();

            if (training == null)
                return NotFound(ApiResponse.CreateError("Training not found", 404));

            return Ok(ApiResponse<TrainingResponseDto>.CreateSuccess(training, "Training retrieved successfully"));
        }

        /// <summary>
        /// Create a new training
        /// </summary>
        /// <param name="model">Training creation data</param>
        /// <returns>Created training details</returns>
        /// <response code="201">Training created successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="409">Training with title already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TrainingResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> CreateTraining([FromBody] CreateTrainingDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var existing = await _context.Trainings.FirstOrDefaultAsync(t => t.Title == model.Title);
            if (existing != null)
                return Conflict(ApiResponse.CreateError("Training with this title already exists", 409));

            var training = new Training
            {
                Title = model.Title,
                Description = model.Description,
                Duration = model.Duration,
                TrainingType = model.TrainingType,
                MaxParticipants = model.MaxParticipants,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();

            var dto = new TrainingResponseDto
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                Duration = training.Duration,
                TrainingType = training.TrainingType,
                MaxParticipants = training.MaxParticipants,
                IsActive = training.IsActive,
                CreatedAt = training.CreatedAt,
                UpdatedAt = training.UpdatedAt,
                EnrolledUsersCount = 0
            };

            return CreatedAtAction(nameof(GetTraining), new { id = training.Id },
                ApiResponse<TrainingResponseDto>.CreateSuccess(dto, "Training created successfully", 201));
        }

        /// <summary>
        /// Update an existing training
        /// </summary>
        /// <param name="id">Training unique identifier</param>
        /// <param name="model">Training update data</param>
        /// <returns>Updated training details</returns>
        /// <response code="200">Training updated successfully</response>
        /// <response code="400">Invalid data or validation errors</response>
        /// <response code="404">Training not found</response>
        /// <response code="409">Training title is already in use</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TrainingResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> UpdateTraining([Required] int id, [FromBody] UpdateTrainingDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.CreateValidationError(ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))));

            var training = await _context.Trainings
                .Include(t => t.UserTrainings)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (training == null)
                return NotFound(ApiResponse.CreateError("Training not found", 404));

            if (training.Title != model.Title)
            {
                var existing = await _context.Trainings.FirstOrDefaultAsync(t => t.Title == model.Title && t.Id != id);
                if (existing != null)
                    return Conflict(ApiResponse.CreateError("Training title is already in use", 409));
            }

            training.Title = model.Title;
            training.Description = model.Description;
            training.Duration = model.Duration;
            training.TrainingType = model.TrainingType;
            training.MaxParticipants = model.MaxParticipants;
            training.IsActive = model.IsActive;
            training.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var dto = new TrainingResponseDto
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                Duration = training.Duration,
                TrainingType = training.TrainingType,
                MaxParticipants = training.MaxParticipants,
                IsActive = training.IsActive,
                CreatedAt = training.CreatedAt,
                UpdatedAt = training.UpdatedAt,
                EnrolledUsersCount = training.UserTrainings.Count()
            };

            return Ok(ApiResponse<TrainingResponseDto>.CreateSuccess(dto, "Training updated successfully"));
        }

        /// <summary>
        /// Delete a training
        /// </summary>
        /// <param name="id">Training unique identifier</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Training deleted successfully</response>
        /// <response code="400">Cannot delete training with enrolled users</response>
        /// <response code="404">Training not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteTraining([Required] int id)
        {
            var training = await _context.Trainings
                .Include(t => t.UserTrainings)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (training == null)
                return NotFound(ApiResponse.CreateError("Training not found", 404));

            if (training.UserTrainings.Any())
                return BadRequest(ApiResponse.CreateError("Cannot delete training with enrolled users"));

            _context.Trainings.Remove(training);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess("Training deleted successfully"));
        }

        /// <summary>
        /// Activate or deactivate a training
        /// </summary>
        /// <param name="id">Training unique identifier</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Status change confirmation</returns>
        /// <response code="200">Training status updated successfully</response>
        /// <response code="404">Training not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateTrainingStatus([Required] int id, [FromQuery] bool isActive)
        {
            var training = await _context.Trainings.FindAsync(id);
            if (training == null)
                return NotFound(ApiResponse.CreateError("Training not found", 404));

            training.IsActive = isActive;
            training.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.CreateSuccess($"Training {(isActive ? "activated" : "deactivated")} successfully"));
        }

        /// <summary>
        /// Get enrolled users for a training
        /// </summary>
        /// <param name="id">Training ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10)</param>
        /// <returns>Paginated list of enrolled users</returns>
        /// <response code="200">Returns enrolled users for the training</response>
        /// <response code="404">Training not found</response>
        [HttpGet("{id}/users")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetTrainingUsers([Required] int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var trainingExists = await _context.Trainings.AnyAsync(t => t.Id == id);
            if (!trainingExists)
                return NotFound(ApiResponse.CreateError("Training not found", 404));

            var users = await _context.UserTrainings
                .Include(ut => ut.User)
                .Where(ut => ut.TrainingId == id)
                .Select(ut => new
                {
                    Id = ut.Id,
                    UserId = ut.UserId,
                    UserName = $"{ut.User.FirstName} {ut.User.LastName}",
                    UserEmail = ut.User.Email,
                    EnrolledAt = ut.EnrolledAt,
                    CompletedAt = ut.CompletedAt,
                    Status = ut.Status,
                    Progress = ut.Progress
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(users, "Training enrolled users retrieved successfully"));
        }

        /// <summary>
        /// Search trainings by title or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching trainings</returns>
        /// <response code="200">Returns matching trainings</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TrainingResponseDto>>), 200)]
        public async Task<IActionResult> SearchTrainings([FromQuery][Required] string searchTerm)
        {
            var trainings = await _context.Trainings
                .Where(t => t.IsActive && (t.Title.Contains(searchTerm) || (t.Description != null && t.Description.Contains(searchTerm))))
                .OrderBy(t => t.Title)
                .Select(t => new TrainingResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Duration = t.Duration,
                    TrainingType = t.TrainingType,
                    MaxParticipants = t.MaxParticipants,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    EnrolledUsersCount = t.UserTrainings.Count()
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<TrainingResponseDto>>.CreateSuccess(trainings, "Trainings search completed successfully"));
        }
    }
}