using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository repository, ILogger<NotificationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<NotificationFilterItem>>> GetCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites)
        {
            try
            {
                var data = (await _repository.GetCategoriesAsync(companies, services, sites)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load notification categories");
                return Failure<List<NotificationFilterItem>>("Failed to load notification categories");
            }
        }

        public async Task<ApiResponse<List<NotificationFilterItem>>> GetServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites)
        {
            try
            {
                var data = (await _repository.GetServicesAsync(companies, categories, sites)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load notification services");
                return Failure<List<NotificationFilterItem>>("Failed to load notification services");
            }
        }

        public async Task<ApiResponse<List<NotificationFilterItem>>> GetCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites)
        {
            try
            {
                var data = (await _repository.GetCompaniesAsync(categories, services, sites)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load notification companies");
                return Failure<List<NotificationFilterItem>>("Failed to load notification companies");
            }
        }

        public async Task<ApiResponse<List<NotificationSiteNode>>> GetSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services)
        {
            try
            {
                var data = (await _repository.GetSitesAsync(companies, categories, services)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load notification sites");
                return Failure<List<NotificationSiteNode>>("Failed to load notification sites");
            }
        }

        public async Task<ApiResponse<NotificationPaginationResponse>> GetNotificationsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, int pageNumber, int pageSize)
        {
            try
            {
                var data = await _repository.GetNotificationsAsync(category, company, service, site, pageNumber, pageSize);
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load notifications");
                return Failure<NotificationPaginationResponse>("Failed to load notifications");
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
                ErrorCode = "ERR_NOTIFICATIONS"
            };
        }
    }
}
