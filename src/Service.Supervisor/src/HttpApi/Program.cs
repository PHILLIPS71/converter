using Serilog;

namespace Giantnodes.Service.Supervisor.HttpApi;

internal static class Program
{
    public static Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        return host.RunWithGraphQLCommandsAsync(args);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
            .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
}