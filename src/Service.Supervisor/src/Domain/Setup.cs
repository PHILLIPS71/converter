using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Service.Supervisor.Domain;

public static class Setup
{
    public static IServiceCollection SetupDomain(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Services
        services.TryAddTransient<ILibraryService, LibraryService>();

        return services;
    }
}