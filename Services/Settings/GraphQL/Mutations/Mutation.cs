using SettingsService.Models;
using SettingsService.Services;

namespace SettingsService.GraphQL.Mutations
{
    public class Mutation
    {
        private readonly ISettingsService _service;

        public Mutation(ISettingsService service)
        {
            _service = service;
        }

        [GraphQLName("updateCompanyDetails")]
        public Task<ApiResponse<CompanyDetailsUpdateResponse>> UpdateCompanyDetails(CompanyDetailsUpdateRequest input)
        {
            return _service.UpdateCompanyDetailsAsync(input);
        }

        [GraphQLName("updateUserPreferences")]
        public Task<ApiResponse<UserPreferencesUpdateResponse>> UpdateUserPreferences(int userId, UserPreferencesUpdateRequest input)
        {
            input.UserId = userId;
            return _service.UpdateUserPreferencesAsync(userId, input);
        }

        [GraphQLName("updateSystemPreferences")]
        public Task<ApiResponse<SystemPreferencesUpdateResponse>> UpdateSystemPreferences(SystemPreferencesUpdateRequest input)
        {
            return _service.UpdateSystemPreferencesAsync(input);
        }

        [GraphQLName("updateNotificationTemplate")]
        public Task<ApiResponse<NotificationTemplateUpdateResponse>> UpdateNotificationTemplate(NotificationTemplateUpdateRequest input)
        {
            return _service.UpdateNotificationTemplateAsync(input);
        }
    }
}
