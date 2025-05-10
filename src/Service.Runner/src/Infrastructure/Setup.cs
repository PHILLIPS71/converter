using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.Pipelines;
using Giantnodes.Service.Runner.Infrastructure.Conversions;
using Giantnodes.Service.Runner.Infrastructure.HostedService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            .AddGiantnodes(options =>
            {
                options
                    .UsingPipelines(configure =>
                    {
                        configure.AddOperation<ConvertOperation>();
                    });
            });

        // System.IO.Abstractions
        services.TryAddSingleton<IFileSystem, FileSystem>();

        // Services
        services.TryAddSingleton<IConversionService, ConversionService>();

        // Hosted Services
        services.AddHostedService<FfMpegDownloaderHostedService>();

        return services;
    }
}