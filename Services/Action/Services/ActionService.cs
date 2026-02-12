using ActionService.Models;
using ActionService.Repositories;

namespace ActionService.Services
{
    public class ActionService : IActionService
    {
        private readonly IActionRepository _repository;
        private readonly ILogger<ActionService> _logger;

        public ActionService(IActionRepository repository, ILogger<ActionService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<ActionFilterItem>>> GetActionCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites)
        {
            try
            {
                var data = (await _repository.GetActionCategoriesAsync(companies, services, sites)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load action categories");
                return Failure<List<ActionFilterItem>>("Failed to load action categories");
            }
        }

        public async Task<ApiResponse<List<ActionFilterItem>>> GetActionCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites)
        {
            try
            {
                var data = (await _repository.GetActionCompaniesAsync(categories, services, sites)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load action companies");
                return Failure<List<ActionFilterItem>>("Failed to load action companies");
            }
        }

        public async Task<ApiResponse<List<ActionFilterItem>>> GetActionServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites)
        {
            try
            {
                var data = (await _repository.GetActionServicesAsync(companies, categories, sites)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load action services");
                return Failure<List<ActionFilterItem>>("Failed to load action services");
            }
        }

        public async Task<ApiResponse<List<ActionSiteNode>>> GetActionSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services)
        {
            try
            {
                var data = (await _repository.GetActionSitesAsync(companies, categories, services)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load action sites");
                return Failure<List<ActionSiteNode>>("Failed to load action sites");
            }
        }

        public async Task<ApiResponse<ActionsPaginationResponse>> GetActionsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, bool isHighPriority, int pageNumber, int pageSize)
        {
            try
            {
                var data = await _repository.GetActionsAsync(category, company, service, site, isHighPriority, pageNumber, pageSize);
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load actions list");
                return Failure<ActionsPaginationResponse>("Failed to load actions");
            }
        }

        public async Task<ApiResponse<ActionItem>> CreateActionAsync(CreateActionRequest request)
        {
            try
            {
                var data = await _repository.CreateActionAsync(request);
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create action");
                return Failure<ActionItem>("Failed to create action");
            }
        }

        private static ApiResponse<T> Success<T>(T data)
        {
            return new ApiResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = "Success",
                ErrorCode = string.Empty
            };
        }

        private static ApiResponse<T> Failure<T>(string message)
        {
            return new ApiResponse<T>
            {
                Data = default,
                IsSuccess = false,
                Message = message,
                ErrorCode = "ERR_ACTIONS"
            };
        }
    }
}
