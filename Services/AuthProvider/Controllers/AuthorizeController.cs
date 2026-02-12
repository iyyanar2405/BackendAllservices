using AuthProvider.Context;
using AuthProvider.Helpers;
using AuthProvider.Models;
using AuthProvider.Models.Tokens;
using AuthProvider.Providers;
using AuthProvider.Services;
using AuthProvider.Settings;
using AuthProvider.Shared.Clients;
using AuthProvider.ViewModel;
using CoreIdentity.API.Identity.Models;
using CoreIdentity.API.Settings;
using Duende.IdentityServer.EntityFramework.Entities;
using Hadvida.EmailService.Models;
using Hadvida.Shared.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Windows.System;
using AuthProvider.DTOs;
using System.ComponentModel.DataAnnotations;

namespace AuthProvider.Controllers
{
    /// <summary>
    /// Authentication and authorization operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Authentication")]
    public class AuthorizeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ClientAppSettings _client;
        private readonly JwtSecurityTokenSettings _jwt;
        private readonly AESManager _aesManager;
        private readonly AuthProviderContext _context;

        /// <summary>
        /// Initializes a new instance of the AuthorizeController
        /// </summary>
        /// <param name="userManager">User manager service</param>
        /// <param name="roleManager">Role manager service</param>
        /// <param name="signInManager">Sign in manager service</param>
        /// <param name="configuration">Application configuration</param>
        /// <param name="client">Client app settings</param>
        /// <param name="jwt">JWT token settings</param>
        /// <param name="context">Database context</param>
        public AuthorizeController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IOptions<ClientAppSettings> client,
            IOptions<JwtSecurityTokenSettings> jwt,
            AuthProviderContext context
            )
        {
            this._context = context;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
            this._configuration = configuration;
            this._client = client.Value;
            this._jwt = jwt.Value;
            userManager.RegisterTokenProvider("AuthProviderToken", new AuthProviderToken(configuration));
            this._aesManager = new AESManager();
        }

        /// <summary>
        /// Confirm user email address with verification token
        /// </summary>
        /// <param name="model">Email confirmation data containing user ID and verification token</param>
        /// <returns>Confirmation result</returns>
        /// <response code="200">Email confirmed successfully</response>
        /// <response code="400">Invalid token or user ID</response>
        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailViewModel model)
        {

            if (model.UserId == null || model.Code == null)
            {
                return BadRequest(new string[] { "Error retrieving information!" });
            }

            var user = await _userManager.FindByIdAsync(model.UserId).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Could not find user!" });


            var result = await _userManager.ConfirmEmailAsync(user, model.Code).ConfigureAwait(false);
            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors.Select(x => x.Description));
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="model">User registration data</param>
        /// <returns>Registration result</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Invalid registration data or validation errors</response>
        /// <response code="409">Email already in use</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            var user = new ApplicationUser(_userManager, _client)
            {
                UserName = model.Email,
                Email = model.Email
               // LastPasswordChanged = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

            if (result.Succeeded)
            {
                //var emailresult = user.SendVerificationEmailAsync(user);

                //if (emailresult.IsCompletedSuccessfully)
                    return Ok();
            }

            return BadRequest(result.Errors.Select(x => x.Description));
        }


        /// <summary>
        /// Authenticate user and generate access token
        /// </summary>
        /// <param name="model">Login credentials including username, password, and optional device code</param>
        /// <returns>Authentication token and user information</returns>
        /// <response code="200">Authentication successful, returns token and user info</response>
        /// <response code="400">Invalid credentials or account locked</response>
        [HttpPost("token")]
        [ProducesResponseType(typeof(ApiResponse<TokenModel>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> TokenJSON([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserName).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Invalid credentials." });

            var tokenModel = new TokenModel()
            {
                HasVerifiedEmail = user.EmailConfirmed
            };

            // Used as user lock
            if (user.LockoutEnabled && user.LockoutEnd>DateTime.UtcNow)
                return BadRequest(new string[] { "This account has been locked." });
            if (await _userManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false))
            {
                var stamp = await _userManager.GenerateConcurrencyStampAsync(user).ConfigureAwait(false);
                //--------------------------------- May Change in future.
                tokenModel.HasVerifiedEmail = true; //must be true or user can't signin.
                //---------------------------------
                tokenModel.TFAEnabled = user.TwoFactorEnabled;
                tokenModel.refreshToken = Guid.NewGuid();

                //Get any valid reset token for the user and attaches that instead of adding the JWT so that a user can reset there password before entering the system.
                var userResetTokens = this._context.aspNetUserTokens.Where(w => w.UserId == user.Id && w.Name == "ResetPassword").ToList();
                foreach (var userResetToken in userResetTokens)
                {

                    var tokenInfo = userResetToken.Value.Split(",");
                    if (tokenInfo.Length == 2)
                    {
                        DateTime.TryParse("", out DateTime baseDateTime);
                        DateTime.TryParse(tokenInfo[1], out DateTime expireStamp);

                        if (expireStamp != baseDateTime)
                            if (expireStamp >= DateTime.UtcNow)
                            {
                                tokenModel.resetToken = tokenInfo[0];
                                break;
                            }
                    }
                }

                bool? deviceSMSCodeStatus = null;
                string tfaPhone = "";
                //Add Summary


                //Two Factor Process start
                //Login Flow--------------
                if (user.TwoFactorEnabled)
                {
                    // initialize deviceCode to null
                    DeviceFlowCodes deviceCode = null; 
                    DeviceFlowCodes deviceFlowCodes = null;

                    if (!string.IsNullOrEmpty(model.DeviceCode))
                    {
                        deviceCode = _context.deviceFlowCodes.Where(w => w.DeviceCode == model.DeviceCode).FirstOrDefault();
                    }

                    List<string> userPhone = new List<string>();
                    if (user.PhoneNumber != null)
                    {
                        tfaPhone = new string(user.PhoneNumber.Where(char.IsDigit).ToArray());

                        if (tfaPhone.Length >= 8)
                        {
                            userPhone.Add(tfaPhone);
                        }
                    }

                    //Prepare and Send Email User Reset for Hadvida Message Service
                    Dictionary<string, string> replacementParams = new Dictionary<string, string>();

                    //Send the SMS TFA Message
                    //Is This device Registered We will know from the devicecode and user entry
                    if (deviceCode != null)
                    {
                        //If Device Link has Expired Removed the DeviceFlow
                        if (deviceCode.Expiration < DateTime.UtcNow)
                        {
                            _context.deviceFlowCodes.Remove(deviceCode);
                            _context.SaveChanges();
                            //Sends email to re-verify code for link.
                            GetTokenModel(tfaPhone, user, tokenModel.refreshToken.ToString(),ref tokenModel);
                            await _userManager.SetAuthenticationTokenAsync(user, "RefreshToken", tokenModel.refreshToken.ToString(), tokenModel.tfaToken + ',' + DateTime.UtcNow.AddMinutes(AppSettings.RefreshTokenExpireTime).ToString()).ConfigureAwait(false);
                        }
                        else
                        {
                            //Logs that the Users has signed in and returns the access_token
                            tokenModel.access_token = await _userManager.GenerateUserTokenAsync(user, "AuthProviderToken", "JWT");
                            await _userManager.ResetAccessFailedCountAsync(user);
                            await _userManager.SetAuthenticationTokenAsync(user, "RefreshToken", tokenModel.refreshToken.ToString(), tokenModel.access_token + ',' + DateTime.UtcNow.AddMinutes(AppSettings.RefreshTokenExpireTime).ToString()).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        GetTokenModel(tfaPhone, user, tokenModel.refreshToken.ToString(), ref tokenModel);                        
                        await _userManager.SetAuthenticationTokenAsync(user, "RefreshToken", tokenModel.refreshToken.ToString(), tokenModel.tfaToken + ',' + DateTime.UtcNow.AddMinutes(AppSettings.RefreshTokenExpireTime).ToString()).ConfigureAwait(false);
                    }

                    //Saves and remove the the deviceflow link if expired(revoked)
                    return Ok(tokenModel);
                }
                else
                {
                    //Logs that the Users has signed in and returns the access_token
                    tokenModel.access_token = await _userManager.GenerateUserTokenAsync(user, "AuthProviderToken", "JWT");
                  //  await _userManager.SetAuthenticationTokenAsync(user, "RefreshToken", tokenModel.refreshToken.ToString(), tokenModel.access_token + ',' + DateTime.UtcNow.AddMinutes(AppSettings.RefreshTokenExpireTime).ToString()).ConfigureAwait(false);
                    await _userManager.ResetAccessFailedCountAsync(user);
                    return Ok(tokenModel);
                }
            }
            else
            {
                //Failed Authentication Update
                await _userManager.AccessFailedAsync(user).ConfigureAwait(false);
            }

            return BadRequest(new string[] { "Invalid login attempt." });
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), 200)]
        /// <summary>
        /// Logs in a user (placeholder method requiring implementation)
        /// </summary>
        /// <param name="model">Token model containing login information</param>
        /// <returns>Authentication result</returns>
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("Login")]
        [Consumes("application/json")]
        public IActionResult LoginUser([FromBody] TokenModel model)
        {
            // This method appears to be incomplete - it should validate credentials and retrieve the user
            // For now, returning a not implemented response to prevent the app from crashing
            return StatusCode(501, "This endpoint needs to be properly implemented");
        }


        /// <summary>
        /// Refresh JWT token with linked Refresh Guid Token
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("refresh-token/{refreshGuid}")]
        public async Task<IActionResult> refreshTokenFromJWT(string refreshGuid)
        {
            // Variable to track if the refresh token is valid
            bool isRefreshTokenValid = false;

            // Get the "Authorization" header value from the request
            HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValue);
            string jwt = authorizationHeaderValue;

            try
            {
                // Remove "Bearer" and spaces from the JWT string
                jwt = jwt.Replace("Bearer", "").Replace(" ", "");
            }
            catch {
                // Set jwt to an empty string if there is an exception
                jwt = "";
            }

            // Parse the refreshGuid parameter to a Guid
            Guid.TryParse(refreshGuid, out Guid userGuid);
            if (string.IsNullOrEmpty(userGuid.ToString()))
                return BadRequest();

            // Get the user based on the parsed userGuid and extracted username from the JWT
            var user = await _userManager.FindByNameAsync(JwtHelper.getUserFromJWT(jwt, AppSettings.GetJwtKey()));

            //Validates when the refreshToken was created
            var isToken = await _userManager.GetAuthenticationTokenAsync(user, "RefreshToken", refreshGuid);
            if (isToken != null)
            {
                // Remove the authentication token for the refresh token
                await _userManager.RemoveAuthenticationTokenAsync(user, "RefreshToken", refreshGuid);

                // Split the stored token into the JWT and expiration timestamp
                var passCodeInfo = isToken.Split(',');
                if (passCodeInfo.Length != 2)
                {
                    return BadRequest();
                }

                // Parse the expiration timestamp from the split token
                DateTime.TryParse("", out DateTime baseDateTime);
                DateTime.TryParse(passCodeInfo[1], out DateTime expireStamp);


                // Check if the expiration timestamp is valid and matches the JWT
                if (expireStamp != baseDateTime && jwt == passCodeInfo[0])
                    if (expireStamp < DateTime.UtcNow)
                        return BadRequest();
                    else
                        isRefreshTokenValid = true;
            }

            // Generate a new concurrency stamp for the user
            var stamp = await _userManager.GenerateConcurrencyStampAsync(user).ConfigureAwait(false);

            if (isRefreshTokenValid)
            {
                // Create a new TokenModel with user details
                TokenModel tokenModel = new TokenModel(user);
                tokenModel.HasVerifiedEmail = true;
                tokenModel.access_token = await _userManager.GenerateUserTokenAsync(user, "AuthProviderToken", "JWT").ConfigureAwait(false);
                tokenModel.refreshToken = Guid.NewGuid();
                // Set the new refresh token as an authentication token for the user
                var result = await _userManager.SetAuthenticationTokenAsync(user, "RefreshToken", tokenModel.refreshToken.ToString(), tokenModel.access_token + ',' + DateTime.UtcNow.AddMinutes(AppSettings.RefreshTokenExpireTime).ToString()).ConfigureAwait(false);
                return Ok(tokenModel);
            }
            else
                return BadRequest();
           
        }



        [HttpPost]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("LogOut")]
        [Consumes("application/json")]
        public async Task<IActionResult> LogOut([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserName).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Invalid credentials." });

            await _signInManager.SignOutAsync().ConfigureAwait(false);
            return Ok(user.UserName + "has been logged out");
        }


        /// <summary>
        /// Forgot email sends an email with a link containing reset token
        /// </summary>
        /// <param name="model">ForgotPasswordViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            try
            {
                // Get the user based on the provided email address
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

                if (user == null)
                    return BadRequest();

                var sysUser = await _userManager.FindByEmailAsync("sys@Hadvida.com").ConfigureAwait(false);


                //Generates a Rest Token and escapes it for the email callback link.
                //var resetcode = await _userManager.GenerateUserTokenAsync(user, "AuthProviderToken", "EncodeUrl");
                var resetcode = await _userManager.GenerateUserTokenAsync(user, "AuthProviderToken", "PassCode");
                string encryptedEscapedToken = Uri.EscapeDataString(resetcode);
                var isTokenSaved = await _userManager.SetAuthenticationTokenAsync(sysUser, "ResetPassword", encryptedEscapedToken, user.Email + ',' + DateTime.UtcNow.AddMinutes(_client.RestTokenExpireTime).ToString());

                var callbackUrl = $"{_client.Url}{_client.ResetPasswordPath}?ref={encryptedEscapedToken}";

                //Creates a user token for api requests
                AuthProviderToken TokenProvider = new AuthProviderToken(_configuration);
                var jwtToken = await TokenProvider.CreateJwtToken(user, _userManager);


                //Create Api Client for Chorus User Information Request
                var portalApiClient = new HadvidaApisClient(jwtToken, HadvidaApiType.Portal);
                var portalRoute = new HadvidaApiRoutes(ApiPortalRoutes.ChorusUserInfo);
                var portalResult = await portalApiClient.SendAsync(portalRoute).ConfigureAwait(false);
                if (portalResult.StatusCode != HttpStatusCode.OK)
                    return BadRequest();


                //Loads Api Response User Information into template
                var chorusUserInfoContent = await portalResult.Content.ReadAsStringAsync();
                dynamic data = (dynamic)JsonConvert.DeserializeObject(chorusUserInfoContent);
                Dictionary<string, string> templateData = new Dictionary<string, string>();
                templateData.Add("First Name", data["firstNm"].Value);
                templateData.Add("Last Name", data["lastNm"].Value);
                templateData.Add("DateTime", DateTime.UtcNow.ToLongDateString());
                templateData.Add("password", callbackUrl);


                //Prepare and Send Email User Reset for Hadvida Message Service
                HadvidaMessage resetMsg = new HadvidaMessage()
                {
                    To = new List<string> { user.Email },
                    MessageTemplateId = 2,
                    MessageType = MessageType.Email,
                    TemplateReplacements = templateData
                };
                var serviceApiClient = new HadvidaApisClient(HadvidaApiType.Services);
                var serviceRoute = new HadvidaApiRoutes(ApiServicesRoutes.SEND_MSG);
                var serviceResult = await serviceApiClient.SendAsync(serviceRoute, resetMsg).ConfigureAwait(false);
            }
            catch
            {
                return Unauthorized();
            }

            return Ok("Check your email for password reset link.");

        }



        /// <summary>
        /// Reset account password with reset token
        /// </summary>
        /// <param name="model">ResetPasswordViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPasswordFromLink([FromBody] SetPasswordViewModel model)
        {
            bool passCodeActive = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            string refItem = "";
            string userName = "";
            refItem = Request.Query.Where(w => w.Key == "ref").First().Value;
            if (string.IsNullOrWhiteSpace(refItem))
                return BadRequest();

            //the sys@Hadvida.com user Manages all the pre auth tokens for use
            var user = await _userManager.FindByEmailAsync("sys@Hadvida.com").ConfigureAwait(false);
            var isToken = await _userManager.GetAuthenticationTokenAsync(user, "ResetPassword", Uri.EscapeDataString(refItem));
            if (isToken != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "ResetPassword", Uri.EscapeDataString(refItem));
                var passCodeInfo = isToken.Split(',');
                if (passCodeInfo.Length != 2)
                {
                    return BadRequest();
                }
                userName = passCodeInfo[0];
                DateTime.TryParse("", out DateTime baseDateTime);
                DateTime.TryParse(passCodeInfo[1], out DateTime expireStamp);

                if (expireStamp != baseDateTime)
                    if (expireStamp < DateTime.UtcNow)
                        return BadRequest();
                    else
                        passCodeActive = true;
            }


            if (!passCodeActive)
            {
                HadvidaBaseToken ebt;
                var decryptToken = "";
                var unescape = Uri.UnescapeDataString(refItem);
                try
                {
                    decryptToken = _aesManager.DecryptString(unescape);
                    ebt = JsonConvert.DeserializeObject<HadvidaBaseToken>(decryptToken);
                    userName = ebt.user;
                    if (ebt == null || ebt.expire < DateTime.UtcNow)
                        return BadRequest();
                }
                catch (Exception)
                {
                    return Unauthorized();
                }
            }


            user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            user.LastPasswordChanged = DateTime.UtcNow;
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest(new string[] { "Invalid User." });
            }

            if(await _userManager.CheckPasswordAsync(user, model.NewPassword).ConfigureAwait(false))
                return BadRequest(new string[] { "New password can't match old password" });

            var jwtToken = await _userManager.GenerateUserTokenAsync(user, "AuthProviderToken", "JWT").ConfigureAwait(false);
            var urlToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetAccessFailedCountAsync(user);
            
            var result = await _userManager.ResetPasswordAsync(user, urlToken, model.NewPassword).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var tokenModel = new TokenModel()
                {
                    HasVerifiedEmail = user.EmailConfirmed,
                    TFAEnabled = user.TwoFactorEnabled,
                    access_token = jwtToken,
                    token_type = "bearer"
                };
                return Ok(tokenModel);
            }

            return BadRequest(result.Errors.Select(x => x.Description));
        }

        /// <summary>
        /// Resend email verification email with token link
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("resendVerificationEmail")]
        public async Task<IActionResult> resendVerificationEmail([FromBody] UserViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Could not find user!" });

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
            var serviceResult = await serviceApiClient.SendAsync(serviceRoute, msg).ConfigureAwait(false);

            if (serviceResult.IsSuccessStatusCode)
                return Ok();
            else
                return BadRequest(serviceResult.Content);

        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims)
            .Union(roleClaims);


            //X509Certificate2 certificate = new X509Certificate2(AppSettings.GetJwtKey(), "password123");
            //var privateKey = certificate.GetRSAPrivateKey();
            //SigningCredentials signingCredentials = new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(AppSettings.JwtExpireTime),
                signingCredentials: null);
            return jwtSecurityToken;
        }


        /// <summary>
        /// Disable TFA
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("setTfa/{userName}/{isEnabled}")]
        public async Task<IActionResult> SetTfa(string userName, bool isEnabled)
        {
            byte[] decodedBytes = Convert.FromBase64String(userName);
            userName = Encoding.UTF8.GetString(decodedBytes);
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null)
                return BadRequest(new string[] { "Could not find user!" });


            var tfaResult = await _userManager.SetTwoFactorEnabledAsync(user, isEnabled).ConfigureAwait(false);
            if (!tfaResult.Succeeded)
                return BadRequest(tfaResult.Errors.Select(x => x.Description));

            return Ok(tfaResult);
        }

        /// <summary>
        /// Reset account password with reset token
        /// </summary>
        /// <param name="model">ResetPasswordViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("setpassword")]
        public async Task<IActionResult> SetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.ToList()));

            var user = await _userManager.FindByNameAsync(model.UserId).ConfigureAwait(false);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest(new string[] { "Invalid credentials." });
            }

            user.LastPasswordChanged = DateTime.UtcNow;
            var resultReset = await _userManager.RemovePasswordAsync(user).ConfigureAwait(false);
            if (!resultReset.Succeeded)
                return BadRequest(resultReset.Errors.Select(x => x.Description));

            var result = await _userManager.AddPasswordAsync(user, model.Password).ConfigureAwait(false);
            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors.Select(x => x.Description));
        }


        /// <summary>
        /// Sends a TFA SMS code to the specified mobile number.
        /// </summary>
        /// <param name="MobileNumber">The mobile number to send the code to.</param>
        /// <param name="Username">The username associated with the mobile number.</param>
        /// <returns>True if the code was sent successfully, false otherwise.</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("SendTfaSMS")]
        public async Task<bool?> SendTfaSMS()
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("uid")?.Value).ConfigureAwait(false);
            string IsSendCode = SendTfaSMSCode(user,"getjwtfromauth").Result;
            if (!string.IsNullOrWhiteSpace(IsSendCode))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        [Route("AddTfaPhone/{userName}/{RefreshToken}/{tfaToken}")]
        public async Task<IActionResult> AddTFAPhone(string userName, string refreshToken, string tfaToken, [FromBody] Dictionary<string, string> data)
        {
            string addNumber = "";
            userName = System.Web.HttpUtility.UrlDecode(userName);
            userName = Base64EncodeDecode.Decode(userName);

            if (data != null && data.Keys.Contains("mobile"))
                addNumber = data.GetValueOrDefault("mobile");
            else
                return BadRequest();

            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            bool IsVerify = false;
            var tfaAccessToken = "";
            var refreshUserToken = "";
            var tokenModel = new TokenModel();

            if (user != null)
            {
                tokenModel.TFAEnabled = user.TwoFactorEnabled;
                tfaAccessToken = await _userManager.GetAuthenticationTokenAsync(user, "TfaToken", tfaToken).ConfigureAwait(true);
                refreshUserToken = await _userManager.GetAuthenticationTokenAsync(user, "RefreshToken", refreshToken).ConfigureAwait(true);
                if (tfaAccessToken != null && refreshUserToken != null)
                {
                    IsVerify = EvaluateTokens(tfaAccessToken, refreshToken);

                    if (IsVerify)
                    {
                        tokenModel.refreshToken = Guid.Parse(refreshToken);

                        //If We are Registering a new phone number to user we stop the verification here
                        //they must now verify the actual number before adding the linked device code.
                        if (!string.IsNullOrWhiteSpace(addNumber))
                        {
                            user.PhoneNumber = addNumber;
                            var phoneResult = _userManager.SetPhoneNumberAsync(user, addNumber).Result;
                            tokenModel.last4 = ExtractLast4NumbersFromString(addNumber);
                            await _userManager.RemoveAuthenticationTokenAsync(user, "TfaToken", tfaToken).ConfigureAwait(false);
                            var newTfaToken = await _userManager.GenerateTwoFactorTokenAsync(user, "AuthProviderToken").ConfigureAwait(false);
                            var tokenResult = await _userManager.SetAuthenticationTokenAsync(user, "TfaToken", newTfaToken, refreshToken + ',' + DateTime.UtcNow.AddMinutes(AppSettings.TfaTokenExpireTime).ToString()).ConfigureAwait(false);
                            await SendTfaSMSCode(user, tokenModel.refreshToken.ToString()).ConfigureAwait(false);
                            return Ok(tokenModel);
                        }
                    }
                }
            }

            return BadRequest();
        }


        [HttpPost]
        [Route("VerifyTFACode/{userName}/{RefreshToken}/{tfaToken}")]
        public async Task<IActionResult> ValidateTFACode(string userName, string refreshToken, string tfaToken)
        {
            string addNumber = "";
            userName = System.Web.HttpUtility.UrlDecode(userName);
            userName = Base64EncodeDecode.Decode(userName);


            var user = await _userManager.FindByNameAsync(userName);
            bool IsVerify = false;
            var tfaAccessToken = "";
            var refreshUserToken = "";
            var tokenModel = new TokenModel();

            if (user != null)
            {
                tokenModel.TFAEnabled = user.TwoFactorEnabled;
                tfaAccessToken = await _userManager.GetAuthenticationTokenAsync(user, "TfaToken", tfaToken);
                refreshUserToken = await _userManager.GetAuthenticationTokenAsync(user, "RefreshToken", refreshToken);
                if (tfaAccessToken != null && refreshUserToken !=null)
                {
                    IsVerify = EvaluateTokens(tfaAccessToken, refreshToken);

                    if (IsVerify)
                    {
                        tokenModel.refreshToken = Guid.Parse(refreshToken);

                       // GetTokenModel(tfaToken, user, refreshToken, ref tokenModel);
                       // var removeToken = await _userManager.RemoveAuthenticationTokenAsync(user, "RefreshToken", refreshToken).ConfigureAwait(false);
                        tokenModel.deviceCode = await CreateDeviceLinkFromCode(user.Id.ToString(), "Desktop", 7, user.PhoneNumber, refreshToken, "");

                        //Creates a user token for api requests
                        AuthProviderToken TokenProvider = new AuthProviderToken(_configuration);
                        if (_userManager != null)
                        {
                            if (!user.PhoneNumberConfirmed)
                            {
                                user.PhoneNumberConfirmed = true;
                                await _userManager.UpdateAsync(user);
                            }
                            //Creates a user token for api requests
                            tokenModel.access_token = await _userManager.GenerateUserTokenAsync(user, "AuthProviderToken", "JWT");
                            var newRefreshtokenResult = await _userManager.SetAuthenticationTokenAsync(user, "RefreshToken", tokenModel.refreshToken.ToString(), tokenModel.access_token + ',' + DateTime.UtcNow.AddMinutes(AppSettings.RefreshTokenExpireTime).ToString()).ConfigureAwait(false);
                            await _userManager.ResetAccessFailedCountAsync(user);

                            if (!string.IsNullOrWhiteSpace(tokenModel.deviceCode) && newRefreshtokenResult.Succeeded)
                                return Ok(tokenModel);
                        }
                    }
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("QRCode/{userName}/{RefreshToken}/{tfaToken}")]
        public async Task<IActionResult> GetQRCode(string userName, string refreshToken, string tfaToken)
        {
            var totp=new TotpAuthProvider();
            userName = System.Web.HttpUtility.UrlDecode(userName);
            userName = Base64EncodeDecode.Decode(userName);
            
            string companyName = "Hadvida";
            string secret = totp.GenerateSecret();
            string otpUrl = totp.GenerateOtpUrl(_client.Url, secret, userName, companyName);

            TfaRegisterModel tfaQr = new TfaRegisterModel();
            tfaQr.qrManualEntryKey = tfaToken;
            tfaQr.qrCodeImageUrl = totp.GenerateQrCode(otpUrl);
            return Ok(tfaQr);
        }



        /// <summary>
        /// NOT DONE NOT DONE NOT DONE NOT DONE!!!!!
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="secret"></param>
        /// <param name="digits"></param>
        /// <param name="issuer"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("totp/{userName}")]
        public async Task<IActionResult> AuthorizeTotp(string userName, [FromQuery] string secret, [FromQuery] int digits, [FromQuery] string issuer, [FromBody] TokenModel model)
        {
            // Get the TOTP code from the request model
            string totpCode = model.tfaToken;
            var totp = new TotpAuthProvider();

            // Validate the TOTP code
            bool isTokenValid = totp.ValidateToken(secret, totpCode);

            if (isTokenValid)
            {
                return Ok(new { success = true });
            }
            else
            {
                return Unauthorized(new { error = "Invalid TOTP code" });
            }
        }


        private async Task<string> SendTfaSMSCode(ApplicationUser user, string refreshToken)
        {
            bool hasPhone = false;
            if (user.PhoneNumber != null)    
            {   
                var tfaPhone = new string(user.PhoneNumber.Where(char.IsDigit).ToArray());
                if (tfaPhone.Length >= 8)
                {
                   hasPhone = true;
                }
            }
        

            //Creates the Tokens
            List<string> userPhone = new List<string>() { user.PhoneNumber };
            Dictionary<string, string> replacementParams = new Dictionary<string, string>();
            string tfaToken = await _userManager.GenerateTwoFactorTokenAsync(user, "AuthProviderToken");
            var tokenResult = await _userManager.SetAuthenticationTokenAsync(user, "TfaToken", tfaToken, refreshToken + ',' + DateTime.UtcNow.AddMinutes(AppSettings.TfaTokenExpireTime).ToString());

            if (tokenResult != null  && hasPhone==false)
            {
                return tfaToken;
            }

            //initialize api client and send the passed in message
            var serviceApiClient = new HadvidaApisClient(HadvidaApiType.Services);
            var serviceRoute = new HadvidaApiRoutes(ApiServicesRoutes.SEND_MSG);
            replacementParams.Add("code", tfaToken);
            HadvidaMessage msg = new HadvidaMessage() { MessageTemplateId = 18, MessageType = MessageType.Mobile, isMultiSite = false, To = userPhone, TemplateReplacements = replacementParams };

            var serviceResult = await serviceApiClient.SendAsync(serviceRoute, msg);

            if (serviceResult.IsSuccessStatusCode && tokenResult != null)
                return tfaToken;
            else
                throw new ArgumentException("Error Generating tfaToken");
        }

        private bool EvaluateTokens(string authToken, string TokensHashMatch)
        {
            if (authToken.Contains(','))
            {
                var tokenData = authToken.Split(',');
                if (tokenData.Length != 2)
                {
                    return false;
                }

                DateTime.TryParse("", out DateTime baseDateTime);
                DateTime.TryParse(tokenData[1], out DateTime expireStamp);

                if (expireStamp != baseDateTime && TokensHashMatch == tokenData[0].Trim())
                    if (expireStamp < DateTime.UtcNow)
                        return false;
                    else
                        return true;
            }

            return false;
        }

        private async Task<string>  CreateDeviceLinkFromCode(string userId, string desc, int daysToExpire, string deviceRef, string sessionId, string data)
        {
            try
            {
                string deviceIdGuid = Guid.NewGuid().ToString();
                DeviceFlowCodes deviceFlowCodes = new DeviceFlowCodes()
                {
                    UserCode = Base64EncodeDecode.Encode(userId + deviceIdGuid),
                    SessionId = sessionId,
                    ClientId = userId,
                    CreationTime = DateTime.UtcNow,
                    Description = desc,
                    Expiration = DateTime.UtcNow.AddDays(daysToExpire),
                    DeviceCode = Base64EncodeDecode.Encode(deviceRef + deviceIdGuid),
                    Data = data
                };

                _context.deviceFlowCodes.Add(deviceFlowCodes);
                _context.SaveChanges();
                return deviceFlowCodes.DeviceCode;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string ExtractLast4NumbersFromString(string input)
        {
            var numberOnlyString= new string(input.Where(char.IsDigit).ToArray());

            if (numberOnlyString.Length<8)
                return "new#";
            else
                return numberOnlyString.Substring(numberOnlyString.Length - 4);
             
        }

        private void GetTokenModel(string tfaPhone, ApplicationUser user, string refreshToken, ref TokenModel tokenModel)
        {
            if (tfaPhone != "0000000000")
            {
                tokenModel.last4 = ExtractLast4NumbersFromString(tfaPhone);
                tokenModel.tfaToken = SendTfaSMSCode(user, refreshToken).Result;

            }
        }
    }
}