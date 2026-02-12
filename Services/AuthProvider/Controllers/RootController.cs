using AuthProvider.Settings;
using AuthProvider.Utility;
using Hadvida.Shared.Managers;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Chorus.Portal.Api.Controllers;

[ApiController]
[Route("/")]
public class RootController : ControllerBase
{
    //[HttpGet]
    //public ActionResult<string> GetVersion()
    //{
    //    string startUpText = "Api Version " + Assembly.GetEntryAssembly().GetName().Version.ToString();
    //    StartupValidator startupValidator = new StartupValidator();
    //    var notFound = startupValidator.CheckAppSettings();

    //    try
    //    {
    //        var certName = AppSettings.CertificateName;
    //        if (!string.IsNullOrWhiteSpace(certName))
    //        {
    //            X509Certificate2 certificate = CertificateManager.GetCertificate(certName);
    //            var aesManager = new AESManager(certificate);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        notFound.Add("Config Certificate - " + ex.Message);
    //    }

    //    //null is returned for a valid connection otherwise the error message is returned
    //    string connectionCheck = startupValidator.CheckDbConnection(AppSettings.GetAuthProviderConnectionString());
    //   if (!string.IsNullOrWhiteSpace(connectionCheck))
    //        notFound.Add("AuthProvider ConnectionString - " +connectionCheck);

    //   if (AppSettings.GetJwtKey().Length==0)
    //    {
    //      notFound.Add("Jwt Key not set");
    //    }


    //    if (notFound.Count > 0)
    //        startUpText += " is not running" + Environment.NewLine + "Start Up Info Missing" + Environment.NewLine;
    //    else
    //        startUpText += " is running";

    //    string missingKeys = "";
    //    foreach (string key in notFound)
    //    {
    //        missingKeys +=  Environment.NewLine + key;
    //    }

    //    startUpText += " " + missingKeys;
    //    return Ok(startUpText);
        
    //}
}