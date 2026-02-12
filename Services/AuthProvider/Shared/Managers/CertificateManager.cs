using System.Security.Cryptography.X509Certificates;

namespace Hadvida.Shared.Managers
{
    public static class CertificateManager
    {
        //Hadvida Internal Certificate is being used as long standing certificate for internal configuration encryption
        public static X509Certificate2 GetCertificate(string subjectName)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, validOnly: false);

            if (certificates.Count > 0)
            {
                X509Certificate2 certificate = certificates[0];
                store.Close();
                return certificate;
            }

            store.Close();
            return null;
        }
    }
}