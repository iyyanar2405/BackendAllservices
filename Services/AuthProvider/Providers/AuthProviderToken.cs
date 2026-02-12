using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthProvider.Helpers;
using AuthProvider.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using AuthProvider.Settings;
using AuthProvider.Models.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using Hadvida.Shared.Managers;

namespace AuthProvider.Providers;

/// <summary>
/// A builder for API resources
/// </summary>
public class AuthProviderToken : IUserTwoFactorTokenProvider<ApplicationUser>
{
    private readonly AESManager _aesManager;

    public IConfiguration _configuration { get; set; }

    public AuthProviderToken(IConfiguration config)
    {
        _configuration = config;
        _aesManager = new AESManager();
    }

    public Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        if (purpose == "EncodeUrl")
        {
            HadvidaBaseToken urlToken = new HadvidaBaseToken(10);
            urlToken.user = user.UserName;
            string encryptedToken = _aesManager.EncryptString(JsonConvert.SerializeObject(urlToken));
            return Task.FromResult(encryptedToken);
        }
        else if(purpose == "PassCode")
        {
           return Task.FromResult(GeneratePassCodes.GenerateTemporaryPassword(10, 2, 2, 2));
        }
        else if (purpose == "TwoFactor")
        {
            return Task.FromResult(GeneratePassCodes.GenerateTemporaryPassword(6, 0, 0, 6));
        }
        else
        {
            JwtSecurityToken jwtSecurityToken = CreateJwtToken(user, manager).Result;
            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return Task.FromResult(token);  
        }
    }


    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        var key = await manager.GetAuthenticatorKeyAsync(user);
        int code;
        if (key == null || !int.TryParse(token, out code))
        {
            return false;
        }

        //  JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user).ConfigureAwait(false);
        return true;
    }


    public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user, UserManager<ApplicationUser> manager)
    {
        var userClaims = await manager.GetClaimsAsync(user).ConfigureAwait(false);
        var roles = await manager.GetRolesAsync(user).ConfigureAwait(false);

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
                new Claim(JwtRegisteredClaimNames.Iss, "Hadvida Inc"),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress)
            }
        .Union(userClaims)
        .Union(roleClaims);


        //X509Certificate2 certificate = new X509Certificate2(AppSettings.GetJwtKey(), "password123");
        //var privateKey = certificate.GetRSAPrivateKey();
        //SigningCredentials signingCredentials = new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: "Hadvida Inc",
            audience: "",
            claims: claims,
            //   expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),

            expires: DateTime.UtcNow.AddMinutes(AppSettings.JwtExpireTime));
        return jwtSecurityToken;
    }


    public async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        var key = await manager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);

        return !string.IsNullOrWhiteSpace(key);
    }


}
