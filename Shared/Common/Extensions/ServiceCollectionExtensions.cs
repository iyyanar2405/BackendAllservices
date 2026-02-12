using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
 

namespace Shared.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add common services here (avoid referencing ASP.NET-specific extensions)
        return services;
    }
}

