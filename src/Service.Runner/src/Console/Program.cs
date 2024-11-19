using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

internal sealed class Program
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
            .ConfigureAppConfiguration((context, configuration) =>
            {
                configuration.AddJsonFile("appsettings.json", false);
                configuration.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true);
                configuration.AddEnvironmentVariables();
                configuration.AddCommandLine(args);
            })
            .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
}