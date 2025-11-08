using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Runner.Persistence.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Giantnodes.Service.Runner.Persistence;

public static class Setup
{
    public static IServiceCollection SetupPersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services
            .AddDbContext<MassTransitDbContext>(options =>
            {
                options
                    .UseNpgsql(configuration.GetConnectionString(name: "DatabaseConnection"), optionsBuilder =>
                    {
                        optionsBuilder.MigrationsHistoryTable("__migrations", MassTransitDbContext.Schema);
                        optionsBuilder.MigrationsAssembly(typeof(MassTransitDbContext).Assembly.FullName);
                    })
                    .UseSnakeCaseNamingConvention();
            });

        services
            .AddOptions<SqlTransportOptions>()
            .Configure(options =>
            {
                options.ConnectionString = configuration.GetConnectionString(name: "TransportConnection");
            });

        services
            .AddHostedService<MigratorHostedService<MassTransitDbContext>>()
            .AddPostgresMigrationHostedService();

        return services;
    }
}
