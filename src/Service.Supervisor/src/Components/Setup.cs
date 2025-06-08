using Giantnodes.Infrastructure.MassTransit;
using Giantnodes.Infrastructure.Pipelines.MassTransit;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Giantnodes.Service.Supervisor.Components;

public static class Setup
{
    public static IServiceCollection SetupComponents(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.SetupMassTransit(configuration, environment);

        return services;
    }

    private static void SetupMassTransit(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services
            .AddMassTransit(options =>
            {
                options.SetKebabCaseEndpointNameFormatter();

                options.AddSqlMessageScheduler();
                options.AddConsumersFromNamespaceContaining<Project.Components>();
                options.AddSagaStateMachinesFromNamespaceContaining<Project.Components>();

                options.AddPipelineOrchestrator();

                options
                    .AddEntityFrameworkOutbox<MassTransitDbContext>(configure =>
                    {
                        configure.UsePostgres();
                        configure.UseBusOutbox();
                    });

                options
                    .SetEntityFrameworkSagaRepositoryProvider(configure =>
                    {
                        configure.ConcurrencyMode = ConcurrencyMode.Optimistic;

                        configure.ExistingDbContext<MassTransitDbContext>();
                        configure.UsePostgres();
                    });

                options
                    .AddConfigureEndpointsCallback((context, name, configure) =>
                    {
                        configure.UseEntityFrameworkOutbox<MassTransitDbContext>(context);
                    });

                options
                    .UsingPostgres((context, config) =>
                    {
                        config.UseSqlMessageScheduler();

                        config.UseGiantnodes(context);

                        config.ConfigureEndpoints(context);
                    });
            });
    }
}