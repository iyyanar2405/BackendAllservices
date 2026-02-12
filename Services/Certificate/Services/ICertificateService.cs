using CertificateService.Models;

namespace CertificateService.Services
{
    public interface ICertificateService
    {
        Task<ApiResponse<List<CertificateListResponse>>> GetCertificateListAsync();
        Task<ApiResponse<CertificateDetailResponse>> GetCertificateDetailsAsync(int certificateId);
        Task<ApiResponse<List<CertificateSiteResponse>>> GetCertificateSitesAsync(int certificateId);
        Task<ApiResponse<PreferenceResponse>> GetPreferencesAsync(string objectType, string objectName, string pageName);
    }
}
