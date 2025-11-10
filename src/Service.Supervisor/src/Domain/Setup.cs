using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Giantnodes.Service.Supervisor.Domain;

public static class Setup
{
    public static IServiceCollection SetupDomain(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Services
        services.TryAddTransient<IDirectoryScanningService, DirectoryScanningService>();
        services.TryAddTransient<ILibraryService, LibraryService>();

        return services;
    }
}
