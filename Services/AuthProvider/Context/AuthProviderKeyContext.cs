using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using AuthProvider.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;
using AuthProvider.Settings;
using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace AuthProvider.Context
{
    public class AuthProviderKeyContext : DbContext
    {
        string authConnection;

        public AuthProviderKeyContext()
        {
            this.Database.SetConnectionString(AppSettings.GetAuthProviderConnectionString());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(AppSettings.GetAuthProviderConnectionString());
            }

            //optionsBuilder.UseSqlServer(AppSettings.GetAuthProviderConnectionString(), options =>
            //{
            //    int maxRetries = 3;
            //    int retryDelayMs = 1000; // 1 second

            //    for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            //    {
            //        try
            //        {
            //            options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);                        
            //            // Enable retry on transient failures
            //            //options.ExecutionStrategy(c => new SqlServerRetryingExecutionStrategy(c, maxRetryCount: 1));
            //            break;
            //        }
            //        catch (SqlException ex)
            //        {
            //            // Log or handle the exception, then retry after a delay
            //            Thread.Sleep(retryDelayMs);
            //        }
            //    }

            //});            
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
        }


        public virtual DbSet<GetEncryptedKeyModel> getKey { get; set; }
       // public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }    

    }
}
