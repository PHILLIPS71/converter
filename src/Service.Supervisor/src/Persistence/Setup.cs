using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Service.Supervisor.Persistence;

public static class Setup
{
    public static IServiceCollection SetupPersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services
            .AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseNpgsql(configuration.GetConnectionString(name: "DatabaseConnection"), optionsBuilder =>
                    {
                        optionsBuilder.MigrationsHistoryTable("__migrations", ApplicationDbContext.Schema);
                        optionsBuilder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    })
                    .UseSnakeCaseNamingConvention();
            });

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
                options.ConnectionString = configuration.GetConnectionString(name: "DatabaseConnection");
                options.Schema = "transport";
            });

        services
            .AddHostedService<MigratorHostedService<ApplicationDbContext>>()
            .AddHostedService<MigratorHostedService<MassTransitDbContext>>()
            .AddPostgresMigrationHostedService();

        return services;
    }
}