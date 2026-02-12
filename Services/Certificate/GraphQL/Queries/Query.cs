using CertificateService.Models;
using CertificateService.Services;

namespace CertificateService.GraphQL.Queries
{
    public class Query
    {
        private readonly ICertificateService _service;

        public Query(ICertificateService service)
        {
            _service = service;
        }

        [GraphQLName("certificates")]
        public Task<ApiResponse<List<CertificateListResponse>>> Certificates()
        {
            return _service.GetCertificateListAsync();
        }

        [GraphQLName("viewCertificateDetails")]
        public Task<ApiResponse<CertificateDetailResponse>> ViewCertificateDetails(int certificateId)
        {
            return _service.GetCertificateDetailsAsync(certificateId);
        }

        [GraphQLName("sitesInScope")]
        public Task<ApiResponse<List<CertificateSiteResponse>>> SitesInScope(int certificateId)
        {
            return _service.GetCertificateSitesAsync(certificateId);
        }

        [GraphQLName("preferences")]
        public Task<ApiResponse<PreferenceResponse>> Preferences(string objectType, string objectName, string pageName)
        {
            return _service.GetPreferencesAsync(objectType, objectName, pageName);
        }
    }
}
