using AuthProvider.Settings;
using Hadvida.Shared.Managers;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace AuthProvider.Utility
{
    public class StartupValidator
    {

        public List<string> CheckAppSettings()
        {
            List<string> notFound = new List<string>();

            try
            {
                var certName = AppSettings.CertificateName;
                //if (!string.IsNullOrWhiteSpace(certName))
                //{
                //    X509Certificate2 certificate = CertificateManager.GetCertificate(certName);
                //    var aesManager = new AESManager(certificate);
                //}
            }
            catch (Exception ex)
            {
                notFound.Add("Config Certificate - " + ex.Message);
            }

            return notFound;
        }

        //Create a method to check the connection string
        public string CheckDbConnection(string connectionString)
        {
            // Create SqlConnection instance to Check the connection
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();
                    return null;
                }
               
            }
            catch (Exception ex)
            {
                // If there's an exception, log it using the ElmahHelper class
                //ElmahHelper.LogError(ex);
                return ex.Message;
            }




}
    }
}
