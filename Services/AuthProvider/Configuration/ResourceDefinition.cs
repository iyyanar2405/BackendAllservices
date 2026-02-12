using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace AuthProvider.Configuration;

internal class ResourceDefinition : ServiceDefinition
{
    public string Scopes { get; set; }
}
