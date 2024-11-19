using Giantnodes.Infrastructure;
using Giantnodes.Service.Runner.Infrastructure.HostedService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Giantnodes.Service.Runner.Infrastructure;

public static class Setup
{
    public static IServiceCollection SetupInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services
            .AddGiantnodes();

        // Hosted Services
        services.AddHostedService<FfMpegDownloaderHostedService>();

        return services;
    }
}