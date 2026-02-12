using FindingsAPI.Gateway.Models.Actions;
using FindingsAPI.Gateway.Repositories;

namespace FindingsAPI.Gateway.Services
{
    public interface IActionsService
    {
        Task<BaseGraphResponse<List<ActionFilterItem>>> GetActionCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites);
        Task<BaseGraphResponse<List<ActionFilterItem>>> GetActionCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites);
        Task<BaseGraphResponse<List<ActionFilterItem>>> GetActionServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites);
        Task<BaseGraphResponse<List<ActionSiteNode>>> GetActionSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services);
        Task<BaseGraphResponse<ActionsPaginationResponse>> GetActionsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, bool isHighPriority, int pageNumber, int pageSize);
    }

    public class ActionsService : IActionsService
    {
        private readonly IActionRepository _repository;
        private readonly ILogger<ActionsService> _logger;

        public ActionsService(IActionRepository repository, ILogger<ActionsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<BaseGraphResponse<List<ActionFilterItem>>> GetActionCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites)
        {
            try
            {
                var data = (await _repository.GetActionCategoriesAsync(companies, services, sites)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting action category filter data");
                return Failure<List<ActionFilterItem>>("Failed to load action categories");
            }
        }

        public async Task<BaseGraphResponse<List<ActionFilterItem>>> GetActionCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites)
        {
            try
            {
                var data = (await _repository.GetActionCompaniesAsync(categories, services, sites)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting action company filter data");
                return Failure<List<ActionFilterItem>>("Failed to load action companies");
            }
        }

        public async Task<BaseGraphResponse<List<ActionFilterItem>>> GetActionServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites)
        {
            try
            {
                var data = (await _repository.GetActionServicesAsync(companies, categories, sites)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting action service filter data");
                return Failure<List<ActionFilterItem>>("Failed to load action services");
            }
        }

        public async Task<BaseGraphResponse<List<ActionSiteNode>>> GetActionSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services)
        {
            try
            {
                var data = (await _repository.GetActionSitesAsync(companies, categories, services)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting action site filter data");
                return Failure<List<ActionSiteNode>>("Failed to load action sites");
            }
        }

        public async Task<BaseGraphResponse<ActionsPaginationResponse>> GetActionsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, bool isHighPriority, int pageNumber, int pageSize)
        {
            try
            {
                var data = await _repository.GetActionsAsync(category, company, service, site, isHighPriority, pageNumber, pageSize);
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting actions list");
                return Failure<ActionsPaginationResponse>("Failed to load actions");
            }
        }

        private static BaseGraphResponse<T> Success<T>(T data)
        {
            return new BaseGraphResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = "Success",
                ErrorCode = string.Empty
            };
        }

        private static BaseGraphResponse<T> Failure<T>(string message)
        {
            return new BaseGraphResponse<T>
            {
                Data = default,
                IsSuccess = false,
                Message = message,
                ErrorCode = "ERR_ACTIONS"
            };
        }
    }
}
