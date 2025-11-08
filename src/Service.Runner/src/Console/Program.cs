using Giantnodes.Service.Runner.Components;
using Giantnodes.Service.Runner.Infrastructure;
using Giantnodes.Service.Runner.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Giantnodes.Service.Runner.Console;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
            return 0;
        }
        catch (Exception)
        {
            return 1;
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services
                    .SetupPersistence(context.Configuration, context.HostingEnvironment)
                    .SetupInfrastructure(context.Configuration, context.HostingEnvironment)
                    .SetupComponents(context.Configuration, context.HostingEnvironment);
            })
            .ConfigureAppConfiguration((context, configuration) =>
            {
                configuration.AddJsonFile("appsettings.json", false);
                configuration.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true);
                configuration.AddEnvironmentVariables();
                configuration.AddCommandLine(args);
            })
            .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
}
