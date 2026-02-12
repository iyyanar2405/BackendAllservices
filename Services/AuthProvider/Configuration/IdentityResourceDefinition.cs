using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace AuthProvider.Configuration;

internal sealed class IdentityResourceDefinition : ResourceDefinition
{
    public IdentityResourceDefinition()
    {
        Profile = "API";
    }
}
