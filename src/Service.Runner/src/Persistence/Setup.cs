using MassTransit;
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
            .AddOptions<SqlTransportOptions>()
            .Configure(options =>
            {
                options.ConnectionString = configuration.GetConnectionString(name: "DatabaseConnection");
                options.Schema = "transport";
            });

        return services;
    }
}