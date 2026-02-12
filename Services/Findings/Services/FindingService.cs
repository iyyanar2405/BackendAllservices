// Services/FindingService.cs
using FindingsAPI.Gateway.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace FindingsAPI.Gateway.Services
{
    public interface IFindingService
    {
        Task<IEnumerable<Finding>> GetFindingsAsync(GetFindingsQuery query);
        Task<Finding> GetFindingByIdAsync(int id, bool includeCompany = false);
        Task<Finding> CreateFindingAsync(CreateFindingCommand command);
        Task<Finding> UpdateFindingAsync(UpdateFindingCommand command);
        Task<Finding> CloseFindingAsync(CloseFindingCommand command);
        Task<BulkUpdateResult> BulkUpdateStatusAsync(BulkUpdateStatusCommand command);
        Task<IEnumerable<Finding>> SearchFindingsAsync(SearchFindingsQuery query);
    }

    public class FindingService : IFindingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FindingService> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;

        public FindingService(
            IUnitOfWork unitOfWork,
            ILogger<FindingService> logger,
            IDistributedCache distributedCache,
            IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<Finding>> GetFindingsAsync(GetFindingsQuery query)
        {
            var cacheKey = $"findings:{JsonSerializer.Serialize(query)}";

            // Try memory cache first
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Finding> cachedFindings))
            {
                _logger.LogDebug("Memory cache hit for findings query");
                return cachedFindings;
            }

            // Try distributed cache
            var distributedCached = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(distributedCached))
            {
                var findings = JsonSerializer.Deserialize<IEnumerable<Finding>>(distributedCached);
                _memoryCache.Set(cacheKey, findings, TimeSpan.FromMinutes(5));
                _logger.LogDebug("Distributed cache hit for findings query");
                return findings;
            }

            try
            {
                var findings = await _unitOfWork.Findings.GetFindingsAsync(query);

                if (query.IncludeCompany)
                {
                    foreach (var finding in findings)
                    {
                        finding.Company = await _unitOfWork.Companies.GetByIdAsync(finding.CompanyId);
                        if (finding.SiteId.HasValue)
                        {
                            finding.Site = await _unitOfWork.Sites.GetByIdAsync(finding.SiteId.Value);
                        }
                    }
                }

                // Cache for 5 minutes
                _memoryCache.Set(cacheKey, findings, TimeSpan.FromMinutes(5));
                await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(findings), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                return findings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting findings from database");
                throw;
            }
        }

        public async Task<Finding> GetFindingByIdAsync(int id, bool includeCompany = false)
        {
            var cacheKey = $"finding:{id}:{includeCompany}";

            // Try memory cache first
            if (_memoryCache.TryGetValue(cacheKey, out Finding cachedFinding))
            {
                _logger.LogDebug("Memory cache hit for finding {Id}", id);
                return cachedFinding;
            }

            // Try distributed cache
            var distributedCached = await _distributedCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(distributedCached))
            {
                var finding = JsonSerializer.Deserialize<Finding>(distributedCached);
                _memoryCache.Set(cacheKey, finding, TimeSpan.FromMinutes(2));
                _logger.LogDebug("Distributed cache hit for finding {Id}", id);
                return finding;
            }

            try
            {
                var finding = await _unitOfWork.Findings.GetByIdAsync(id);
                if (finding == null)
                    return null;

                if (includeCompany)
                {
                    finding.Company = await _unitOfWork.Companies.GetByIdAsync(finding.CompanyId);
                    if (finding.SiteId.HasValue)
                    {
                        finding.Site = await _unitOfWork.Sites.GetByIdAsync(finding.SiteId.Value);
                    }
                }

                // Cache for 2 minutes
                _memoryCache.Set(cacheKey, finding, TimeSpan.FromMinutes(2));
                await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(finding), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });

                return finding;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting finding {Id} from database", id);
                throw;
            }
        }

        public async Task<Finding> CreateFindingAsync(CreateFindingCommand command)
        {
            try
            {
                var finding = new Finding
                {
                    Title = command.Title,
                    Category = command.Category,
                    CompanyId = command.CompanyId,
                    SiteId = command.SiteId,
                    Services = command.Services,
                    Description = command.Description,
                    Severity = command.Severity,
                    CreatedBy = command.CreatedBy,
                    Status = "Open", // Default status
                    OpenDate = DateTime.UtcNow,
                    // Set other properties as needed
                };

                await _unitOfWork.Findings.AddAsync(finding);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate relevant caches
                InvalidateFindingsCache();

                return finding;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating finding");
                throw;
            }
        }
        
        public async Task<Finding> UpdateFindingAsync(UpdateFindingCommand command)
        {
            try
            {
                var finding = await _unitOfWork.Findings.GetByIdAsync(command.FindingId);
                if (finding == null)
                {
                    throw new System.Collections.Generic.KeyNotFoundException($"Finding with ID {command.FindingId} not found");
                }

                finding.Status = command.Status;
                finding.Response = command.Response;
                finding.DueDate = command.DueDate;
                finding.UpdatedBy = command.UpdatedBy;

                await _unitOfWork.Findings.UpdateAsync(finding);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate relevant caches
                InvalidateFindingsCache();

                return finding;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating finding {Id}", command.FindingId);
                throw;
            }
        }

        public async Task<Finding> CloseFindingAsync(CloseFindingCommand command)
        {
            try
            {
                var finding = await _unitOfWork.Findings.GetByIdAsync(command.FindingId);
                if (finding == null)
                {
                    throw new System.Collections.Generic.KeyNotFoundException($"Finding with ID {command.FindingId} not found");
                }

                finding.Status = "Closed";
                finding.ClosureNotes = command.ClosureNotes;
                finding.ClosedBy = command.ClosedBy;

                await _unitOfWork.Findings.UpdateAsync(finding);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate relevant caches
                InvalidateFindingsCache();

                return finding;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing finding {Id}", command.FindingId);
                throw;
            }
        }

        public async Task<BulkUpdateResult> BulkUpdateStatusAsync(BulkUpdateStatusCommand command)
        {
            try
            {
                var result = new BulkUpdateResult
                {
                    FailedIds = new List<int>()
                };

                foreach (var findingId in command.FindingIds)
                {
                    try
                    {
                        var finding = await _unitOfWork.Findings.GetByIdAsync(findingId);
                        if (finding != null)
                        {
                            finding.Status = command.NewStatus;
                            finding.Response = command.Reason;
                            finding.UpdatedBy = command.UpdatedBy;

                            await _unitOfWork.Findings.UpdateAsync(finding);
                            result.UpdatedCount++;
                        }
                        else
                        {
                            result.FailedIds.Add(findingId);
                            result.FailedCount++;
                        }
                    }
                    catch (Exception)
                    {
                        result.FailedIds.Add(findingId);
                        result.FailedCount++;
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Invalidate relevant caches
                InvalidateFindingsCache();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating findings status");
                throw;
            }
        }

        public async Task<IEnumerable<Finding>> SearchFindingsAsync(SearchFindingsQuery query)
        {
            var cacheKey = $"search:{JsonSerializer.Serialize(query)}";
            
            // Try cache first
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Finding> cachedFindings))
            {
                _logger.LogDebug("Cache hit for search query");
                return cachedFindings;
            }

            try
            {
                var findings = await _unitOfWork.Findings.SearchAsync(query);

                // Cache the result
                _memoryCache.Set(cacheKey, findings, TimeSpan.FromMinutes(5));
                
                return findings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching findings");
                throw;
            }
        }
        
        private void InvalidateFindingsCache()
        {
            // Remove all findings-related cache entries
            // Note: IMemoryCache doesn't have GetKeys, so we'll skip this for now
            // var cacheKeys = _cache.GetKeys<string>().Where(k => k.StartsWith("findings:"));
            // foreach (var key in cacheKeys)
            // {
            //     _cache.Remove(key);
            // }
        }
    }
}