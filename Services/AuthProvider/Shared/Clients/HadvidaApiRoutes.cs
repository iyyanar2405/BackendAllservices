using System.ComponentModel;
using AuthProvider.Helpers;

namespace AuthProvider.Shared.Clients
{
    public enum ApiPortalRoutes
    {
        [Description("api/user/ChorusUserInfo,GET")]
        ChorusUserInfo = 0,
        [Description("api/AuthProvider/GetUserTFAPhone,GET")]
        UserTfaPhone= 1,
        [Description("api/AuthProvider/GetUserAccess,GET")]
        GetUserAccess = 2
    }

    public enum ApiServicesRoutes
    {
        [Description("api/Message/Send,POST")]
        SEND_MSG = 0,
    }
    public class HadvidaApiRoutes
    {
        public string route { get; set; }
        public string method { get; set; }



        public HadvidaApiRoutes(ApiPortalRoutes apiRoutes)
        {
            string routeStr = EnumHelper.GetEnumDescription(apiRoutes);
            var routeMethod = routeStr.Split(',');
            route = routeMethod[0];
            method = routeMethod[1];
        }

        public HadvidaApiRoutes(ApiServicesRoutes apiRoutes)
        {
            string routeStr = EnumHelper.GetEnumDescription(apiRoutes);
            var routeMethod = routeStr.Split(',');
            route = routeMethod[0];
            method = routeMethod[1];
        }

    }

}
