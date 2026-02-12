using CertificateService.Models;
using CertificateService.Models.Rest;
using RestCertificateListResponse = CertificateService.Models.Rest.CertificateListResponse;

namespace CertificateService.Services
{
    public interface ICertificateRestService
    {
        Task<ApiResponse<RestCertificateListResponse>> GetCertificateListAsync(CertificateListRequest request);
        Task<ApiResponse<RestCertificateListResponse>> SearchCertificatesAsync(CertificateSearchRequest request);
        Task<ApiResponse<CertificateDetailFullResponse>> GetCertificateDetailsAsync(int certificateId);
        Task<ApiResponse<CertificateStatusUpdateResponse>> UpdateCertificateStatusAsync(int certificateId, UpdateCertificateStatusRequest request);
    }
}
