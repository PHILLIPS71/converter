using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.GraphQL;
using Giantnodes.Service.Supervisor.Components;
using Giantnodes.Service.Supervisor.Persistence;

namespace Giantnodes.Service.Supervisor.HttpApi;

internal sealed class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    var origins = _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                    if (origins == null || origins.Length == 0)
                        throw new ConfigurationException("Cors.AllowedOrigins");

                    builder
                        .WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

        services
            .SetupPersistence(_configuration, _environment)
            .SetupComponents(_configuration, _environment);

        services
            .AddGraphQLServer()
            .ModifyOptions(options => options.DefaultFieldBindingFlags = FieldBindingFlags.Default)
            .AddGiantnodesConfiguration()
            .AddGlobalObjectIdentification()
            .AddMutationConventions()
            .AddHttpApiTypes()
            .AddProjections()
            .AddPagingArguments()
            .AddFiltering()
            .AddSorting()
            .InitializeOnStartup();
    }

    public void Configure(IApplicationBuilder app)
    {
        if (!_environment.IsDevelopment())
            app.UseHttpsRedirection();

        app
            .UseCors()
            .UseRouting()
            .UseEndpoints(endpoint => endpoint.MapGraphQL());
    }
}