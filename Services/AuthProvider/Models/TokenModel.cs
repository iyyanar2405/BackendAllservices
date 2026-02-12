using AuthProvider.Models;
using AuthProvider.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreIdentity.API.Identity.Models
{
    public class TokenModel
    {
        public bool? HasVerifiedEmail { get; set; }
        public bool? TFAEnabled { get; set; }
        public string? access_token { get; set; }

        public string? token_type { get; set; } = "bearer";

        public int expires_in { get; set; } = AppSettings.JwtExpireTime;

        public string issued { get; set; } = DateTime.UtcNow.ToString();

        public string expires { get; set; } = DateTime.UtcNow.AddMinutes(AppSettings.JwtExpireTime).ToString();

        public Guid refreshToken { get; set; }

        public string resetToken { get; set; }

        public string tfaToken { get; set; }

        public string deviceCode { get; set; }

        public string last4 { get; set; }

        public TokenModel(ApplicationUser applicationUser)
        {
            HasVerifiedEmail = applicationUser.EmailConfirmed;
            TFAEnabled = applicationUser.TwoFactorEnabled;

        }

        public TokenModel() { }
    }
}
