using AuthProvider.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Windows.UI.Xaml;

namespace AuthProvider.Helpers
{
    public class JwtHelper
    {
        public static string getUserFromJWT(string jwtToken, byte[] signingKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
           // X509Certificate2 certificate = new X509Certificate2(AppSettings.GetJwtKey(), "password123");
            // Set the validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = null,
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                // Validate and parse the JWT
                var claimsPrincipal = tokenHandler.ReadJwtToken(jwtToken);
                return claimsPrincipal.Claims.Where(w => w.Type == "sub").First().Value;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during token validation
                throw new SecurityTokenException("Invalid token", ex);
            }
        }
    }
}
