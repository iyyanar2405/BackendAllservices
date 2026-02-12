using Microsoft.AspNetCore.Http;

namespace AuthProvider.Extensions;

internal interface IAbsoluteUrlFactory
{
    string GetAbsoluteUrl(string path);
    string GetAbsoluteUrl(HttpContext context, string path);
}
