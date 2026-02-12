using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using AuthProvider.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;
using AuthProvider.Settings;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthProvider.Context
{
    public class AuthProviderContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        string authConnection;

        public AuthProviderContext(DbContextOptions options, IOptions<OperationalStoreOptions?> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
            authConnection = AppSettings.GetAuthProviderConnectionString();
           
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
        }


        public virtual DbSet<GetEncryptedKeyModel> getKey { get; set; }
       // public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }    

        public virtual DbSet<IdentityUserToken<string>> aspNetUserTokens { get; set; }

        public virtual DbSet<DeviceFlowCodes> deviceFlowCodes { get; set; }

    }
}
