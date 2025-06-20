﻿using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Infrastructure.MassTransit;
using Giantnodes.Infrastructure.Pipelines;
using Giantnodes.Infrastructure.Pipelines.MassTransit;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Infrastructure.HostedServices;
using Giantnodes.Service.Supervisor.Infrastructure.Repositories;
using Giantnodes.Service.Supervisor.Infrastructure.Services;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Giantnodes.Service.Supervisor.Infrastructure;

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
                    .UsingUow(configure =>
                    {
                        configure
                            .TryAddProvider<EntityFrameworkUnitOfWork<ApplicationDbContext>>()
                            .TryAddInterceptor<PublishUnitOfWorkInterceptor>();
                    });

                options
                    .UsingPipelines(configure =>
                    {
                        configure.UseMassTransit();
                    });
            });

        // System.IO.Abstractions
        services.TryAddSingleton<IFileSystem, FileSystem>();
        services.TryAddSingleton<IFileSystemWatcherFactory, FileSystemWatcherFactory>();

        // Repositories
        services.TryAddTransient<IDirectoryRepository, DirectoryRepository>();
        services.TryAddTransient<IDistributionRepository, DistributionRepository>();
        services.TryAddTransient<IFileRepository, FileRepository>();
        services.TryAddTransient<IFileSystemEntryRepository, FileSystemEntryRepository>();
        services.TryAddTransient<ILibraryRepository, LibraryRepository>();
        services.TryAddTransient<IPipelineRepository, PipelineRepository>();

        // Services
        services.TryAddSingleton<IFileSystemMonitoringService, FileSystemMonitoringService>();
        services.TryAddSingleton<ILibraryMonitoringService, LibraryMonitoringService>();
        services.TryAddScoped<IPipelineExecutionService, PipelineExecutionService>();

        // Hosted Services
        services.AddHostedService<LibraryMonitoringBackgroundService>();

        return services;
    }
}