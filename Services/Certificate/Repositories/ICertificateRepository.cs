using CertificateService.Models;

namespace CertificateService.Repositories
{
    public interface ICertificateRepository
    {
        Task<IReadOnlyList<CertificateService.Models.CertificateListResponse>> GetCertificateListAsync();
        Task<CertificateDetailResponse?> GetCertificateDetailsAsync(int certificateId);
        Task<CertificateService.Models.Rest.CertificateListPageData> GetCertificateListPageAsync(
            CertificateService.Models.Rest.CertificateListRequest request);
        Task<CertificateService.Models.Rest.CertificateListPageData> SearchCertificatesAsync(
            CertificateService.Models.Rest.CertificateSearchRequest request);
        Task<CertificateService.Models.Rest.CertificateDetailFullResponse?> GetCertificateDetailsFullAsync(int certificateId);
        Task<IReadOnlyList<CertificateSiteResponse>> GetCertificateSitesAsync(int certificateId);
        Task<PreferenceResponse?> GetPreferencesAsync(string objectType, string objectName, string pageName);
        Task<CertificateService.Models.Rest.CertificateStatusUpdateResponse?> UpdateCertificateStatusAsync(
            int certificateId,
            CertificateService.Models.Rest.UpdateCertificateStatusRequest request);
    }
}
