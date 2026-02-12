using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace AuthProvider.Settings
{
    public static class AppSettings
    {
        public static string AuthProviderKeyId = "2147483647";
        private static byte[]? jwtKey { get; set; } = null;
        private static byte[]? preAuthKey { get; set; } = null;
        private static string chorusApiBaseUrl { get; set; }

        private static string AuthProvidedConnectionString { get; set; }

        private static string env { get; set; }

        public static bool AllowSwagger { get; set; }

        public static bool AllowEmails { get; set; } = false;

        public static string SwaggerIPRangeList { get; set; }

        public static string HadvidaEmailServiceUrl { get; set; }
        public static int RefreshTokenExpireTime { get; set; }
        public static int JwtExpireTime { get; set; }
        public static int SessionExpireTime { get; set; }
        public static int TfaTokenExpireTime { get; set; }
        public static int MaxFailedAccessAttempts { get; set; }

        public static string CertificateName { get; set; }

        public static void SetJwtKey(byte[] key)
        {
            jwtKey = key;
        }

        public static byte[]? GetJwtKey()
        {
            return jwtKey;
        }

        public static void SetPreAuthKey(byte[] key)
        {
            preAuthKey = key;
        }

        public static byte[]? GetPreAuthKey()
        {
            return preAuthKey;
        }

        public static void SetAuthProviderConnectionString(string connection)
        {
            AuthProvidedConnectionString = connection;
        }

        public static String GetAuthProviderConnectionString()
        {
            return AuthProvidedConnectionString;
        }

        public static void Setenv(string environment)
        {
            env = environment;
        }

        public static String Getenv()
        {
            return env;
        }

        public static void SetChorusApiBaseUrl(string baseurl)
        {
            chorusApiBaseUrl = baseurl;
        }

        public static string GetChorusApiBaseUrl()
        {
            return chorusApiBaseUrl;
        }


    }
}
