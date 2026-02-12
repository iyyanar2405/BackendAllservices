using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

public class WhiteListMiddleware : IMiddleware
{
    private readonly List<IPAddress> _whitelistedIps = new List<IPAddress>();
    private readonly string _rawList;

    public WhiteListMiddleware(IConfiguration config)
    {
        _rawList = config.GetSection("WhiteList").Value;

        if (string.IsNullOrWhiteSpace(_rawList))
            _rawList = "*";

        if (_rawList != "*")
        {
                _whitelistedIps = _rawList.Split(',').ToList().Select(s => {
                    IPAddress parseIP = null;
                    IPAddress.TryParse(s, out parseIP);
                    return parseIP;
                        }).ToList();
        }
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_rawList != "*")
        {
            var clientIp = context.Connection.RemoteIpAddress;

            if (!_whitelistedIps.Contains(clientIp))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
        }

        await next(context);
    }
}
