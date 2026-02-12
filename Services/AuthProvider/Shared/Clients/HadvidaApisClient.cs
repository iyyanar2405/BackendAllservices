using AuthProvider.Helpers;
using AuthProvider.Settings;
using AuthProvider.Shared.Clients;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using AuthProvider.Shared.Clients;

namespace AuthProvider.Services
{

    public enum HadvidaApiType
    {
        Portal = 0,
        App = 1,
        Services = 2,
        PdfFiller = 3,
        AuthProvider = 4
    }


    public class HadvidaApisClient
    {
        //const string SEND_MSG = "api/Message/Send";
        private HttpClient _httpClient = new HttpClient();
        private Uri _uri;
        public string _clientApiBaseUrl;

        HadvidaApisClient()
        {
            HttpClient httpClient = new HttpClient();
            _httpClient = httpClient;
        }

        public HadvidaApisClient(JwtSecurityToken token, HadvidaApiType apiType)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = loadApiBaseUrl(apiType);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var hdrToken = new JwtSecurityTokenHandler().WriteToken(token);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hdrToken);
            _httpClient = httpClient;
        }

        public HadvidaApisClient(HadvidaApiType apiType)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = loadApiBaseUrl(apiType);//new Uri(AppSettings.GetChorusApiBaseUrl());
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient = httpClient;
        }

        private Uri loadApiBaseUrl(HadvidaApiType apiType)
        {
            switch (apiType)
            {
                case HadvidaApiType.Portal:
                    _clientApiBaseUrl = AppSettings.GetChorusApiBaseUrl();
                    break;
                case HadvidaApiType.App:
                    break;
                case HadvidaApiType.Services:
                    _clientApiBaseUrl = AppSettings.HadvidaEmailServiceUrl;
                    break;
                default:
                    break;
            }
            return new Uri(_clientApiBaseUrl);
        }

        public async Task<HttpResponseMessage> SendAsync(HadvidaApiRoutes? path, object? jsonObj=null)
        {

           var uri  = new Uri(Path.Combine(_clientApiBaseUrl, path.route));

            var pathRouteToSkip = EnumHelper.GetEnumDescription(ApiServicesRoutes.SEND_MSG).ToString().Split(',')[0];
            switch (path.method)
            {
                case "POST":
                    if (!AppSettings.AllowEmails && path.route == pathRouteToSkip)
                        return new HttpResponseMessage();
                    else
                        return await _httpClient.PostAsync(uri.ToString(),
                        new StringContent(JsonConvert.SerializeObject(jsonObj), System.Text.Encoding.UTF8, "application/json"));

                    //Default does the Get
                default:
                        return await _httpClient.GetAsync(uri.ToString());
            }

            
        }

    }
}
