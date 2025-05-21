using Giantnodes.Infrastructure.MassTransit;
using Giantnodes.Infrastructure.Pipelines.MassTransit;
using Giantnodes.Service.Runner.Persistence.DbContexts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Giantnodes.Service.Runner.Components;

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

                options.AddPipelineWorker();

                options
                    .AddJobSagaStateMachines(configure => configure.FinalizeCompleted = true)
                    .EntityFrameworkRepository(configure =>
                    {
                        configure.ConcurrencyMode = ConcurrencyMode.Optimistic;

                        configure.ExistingDbContext<MassTransitDbContext>();
                        configure.UsePostgres();
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