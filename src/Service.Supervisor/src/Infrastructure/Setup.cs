using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Infrastructure.MassTransit;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Infrastructure.HostedServices;
using Giantnodes.Service.Supervisor.Infrastructure.Repositories;
using Giantnodes.Service.Supervisor.Infrastructure.Services;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Service.Identity.Infrastructure;

public static class Setup
{
    public static IServiceCollection SetupInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services
            .AddGiantnodes(options =>
            {
                options
                    .UsingUow(configure =>
                    {
                        configure
                            .TryAddProvider<EntityFrameworkUnitOfWork<ApplicationDbContext>>()
                            .TryAddInterceptor<PublishUnitOfWorkInterceptor>();
                    });
            });

        // System.IO.Abstractions
        services.TryAddSingleton<IFileSystem, FileSystem>();
        services.TryAddSingleton<IFileSystemWatcherFactory, FileSystemWatcherFactory>();

        // Repositories
        services.TryAddTransient<ILibraryRepository, LibraryRepository>();
        services.TryAddTransient<IDirectoryRepository, DirectoryRepository>();

        // Services
        services.TryAddSingleton<IFileSystemMonitoringService, FileSystemMonitoringService>();
        services.TryAddSingleton<ILibraryMonitoringService, LibraryMonitoringService>();

        // Hosted Services
        services.AddHostedService<LibraryMonitoringBackgroundService>();

        return services;
    }
}