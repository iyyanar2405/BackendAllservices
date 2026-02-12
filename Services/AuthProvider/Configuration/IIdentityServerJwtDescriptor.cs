using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace AuthProvider.Configuration;

internal interface IIdentityServerJwtDescriptor
{
    IDictionary<string, ResourceDefinition> GetResourceDefinitions();
}
