using Giantnodes.Infrastructure.MassTransit;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Service.Supervisor.Components;

public static class Setup
{
    public static IServiceCollection SetupComponents(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.SetupMassTransit(configuration, environment);

        return services;
    }

    private static void SetupMassTransit(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services
            .AddMassTransit(options =>
            {
                options.SetKebabCaseEndpointNameFormatter();

                options.AddSqlMessageScheduler();
                options.AddConsumersFromNamespaceContaining<Project.Components>();

                options
                    .AddEntityFrameworkOutbox<MassTransitDbContext>(configure =>
                    {
                        configure.UsePostgres();
                        configure.UseBusOutbox();
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