using AuthProvider.Services;
using AuthProvider.Shared.Clients;
using CoreIdentity.API.Settings;
using Hadvida.EmailService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Windows.System;

namespace AuthProvider.Models
{
    public class ApplicationUser : IdentityUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ClientAppSettings _client;

        public ApplicationUser() {}

        public DateTime? LastPasswordChanged { get; set; }

        public ApplicationUser(UserManager<ApplicationUser> userManager, ClientAppSettings client)
        {
            _userManager = userManager;
            _client = client;
        }

        public ApplicationUser(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<HttpResponseMessage> SendVerificationEmailAsync(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            var callbackUrl = $"{_client.Url}{_client.EmailConfirmationPath}?uid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";

            List<string> email = new List<string>();
            email.Add(user.Email);
            Dictionary<string, string> replacementParams = new Dictionary<string, string>();
            replacementParams.Add("EmailVerifyLink", callbackUrl);

            //Prepare and Send Email User Reset for Hadvida Message Service
            HadvidaMessage msg = new HadvidaMessage()
            {
                MessageTemplateId = 20,
                MessageType = MessageType.Email,
                isMultiSite = false,
                To = email,
                TemplateReplacements = replacementParams
            };
            var serviceApiClient = new HadvidaApisClient(HadvidaApiType.Services);
            var serviceRoute = new HadvidaApiRoutes(ApiServicesRoutes.SEND_MSG);
            return await serviceApiClient.SendAsync(serviceRoute, msg).ConfigureAwait(false);
        }

    }
}