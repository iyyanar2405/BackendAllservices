using AuthProvider.Settings;
using CoreIdentity.API.Settings;
using Hadvida.EmailService.Models;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AuthProvider.Models.Tokens
{
    public class HadvidaBaseToken
    {
        public string user;

        public DateTime expire;

        public HadvidaBaseToken(int addExpiresInMin)
        {
            expire = DateTime.UtcNow.AddMinutes(addExpiresInMin);
        }
    }
}
