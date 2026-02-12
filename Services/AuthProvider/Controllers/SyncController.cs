using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthProvider.ViewModel;
using AuthProvider.Models;
using System.Text;
using AuthProvider.Providers;
using Newtonsoft.Json;
using System.Data;
using AuthProvider.Settings;
using AuthProvider.Helpers;
using AuthProvider.Services;
using AuthProvider.Shared.Clients;
using CoreIdentity.API.Settings;
using Duende.IdentityServer.Services;
using Microsoft.Extensions.Options;

namespace AuthProvider.Controllers
{
    //[Produces("application/json")]
    [Route("api/sync")]
    public class SyncController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ClientAppSettings _client;
        public IConfiguration _configuration { get; set; }

        public SyncController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<ClientAppSettings> client,
            IConfiguration configuration
            )
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
            this._client = client.Value;
            userManager.RegisterTokenProvider("AuthProviderToken", new AuthProviderToken(configuration));
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[ProducesResponseType(typeof(IdentityResult), 200)]
        //[ProducesResponseType(typeof(ApplicationUser), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("Chorus/User/{userId}/{password}")]
        public async Task<IActionResult> SyncChorusUser(string userId, string password)
        {
            bool isNewUser = false;
            string phoneNumber = "";
            string autoCreatedTmpPass;

            userId = Encoding.ASCII.GetString(Convert.FromBase64String(userId));
            password = Encoding.ASCII.GetString(Convert.FromBase64String(password));
            autoCreatedTmpPass = password;

            var user = _userManager.Users
                                .Where(user => user.UserName == userId)
                                .FirstOrDefault();


            //generates a new password if none where passed in.
            if (password == "none")
                autoCreatedTmpPass = GeneratePassCodes.GenerateTemporaryPassword(8, 2, 2, 2);

            if (user == null)
            {
                isNewUser = true;
                user = new ApplicationUser(_userManager, _client) { UserName = userId, Email = userId, Id = Guid.NewGuid().ToString() };

                IdentityResult result = await _userManager.CreateAsync(user, autoCreatedTmpPass).ConfigureAwait(false);
                if (!result.Succeeded)
                    return BadRequest();
                else
                    user = _userManager.Users
                                .Where(user => user.UserName == userId)
                                .FirstOrDefault();
            }

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            if (!roles.Where(w => w == "Chorus Site User").Any())
            {
                IdentityResult result = await _userManager.AddToRoleAsync(user, "Chorus Site User").ConfigureAwait(false);
                if (!result.Succeeded)
                    return BadRequest();
            }
            //Chorus Site User
            AuthProviderToken TokenProvider = new AuthProviderToken(_configuration);
            var token = TokenProvider.CreateJwtToken(user, _userManager).Result;

            //Create Api Client for Chorus User Information Request
            var portalApiClient = new HadvidaApisClient(token, HadvidaApiType.Portal);
            var route = new HadvidaApiRoutes(ApiPortalRoutes.UserTfaPhone);
            var tfaPhoneResult = await portalApiClient.SendAsync(route).ConfigureAwait(false);

            UserTfaInfo userTfaInfo = new UserTfaInfo() { isTfaLogin=false, isTwoFactor=false, methodId=0, mobNum=""};
            if (tfaPhoneResult.IsSuccessStatusCode)
            {
                var content = await tfaPhoneResult.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                    userTfaInfo = JsonConvert.DeserializeObject<UserTfaInfo>(content);
            }
            

            route = new HadvidaApiRoutes(ApiPortalRoutes.GetUserAccess);
            var userAccessResult = await portalApiClient.SendAsync(route).ConfigureAwait(false);

            List<ChorusAccessViewModel>? chorusUserAccess = new List<ChorusAccessViewModel>();
            if (userAccessResult.IsSuccessStatusCode)
            {
                var data = await userAccessResult.Content.ReadAsStringAsync();
                chorusUserAccess = JsonConvert.DeserializeObject<List<ChorusAccessViewModel>>(data);

                IdentityResult result;
                result = await _userManager.SetTwoFactorEnabledAsync(user, userTfaInfo.isTwoFactor).ConfigureAwait(false);
                if (!result.Succeeded)
                    return BadRequest();

                if (isNewUser && userTfaInfo.mobNum!="" || (string.IsNullOrWhiteSpace(user.PhoneNumber) && userTfaInfo.mobNum!=""))
                {
                    result = await _userManager.SetPhoneNumberAsync(user, userTfaInfo.mobNum).ConfigureAwait(false);
                    if (!result.Succeeded)
                        return BadRequest();
                }

                if (autoCreatedTmpPass == password)
                {       
                    //Creates Passcode for Users Password Reset Update
                    var sysUser = await _userManager.FindByEmailAsync("sys@Hadvida.com").ConfigureAwait(false);
                    string encryptedEscapedToken = Uri.EscapeDataString(autoCreatedTmpPass);
                    var isTokenSaved = await _userManager.SetAuthenticationTokenAsync(sysUser, "ResetPassword", encryptedEscapedToken, user.Email + ',' + DateTime.UtcNow.AddMinutes(_client.RestTokenExpireTime).ToString());
                }

            }
            else
                return BadRequest();


            return Ok();

        }

       
    }
}
