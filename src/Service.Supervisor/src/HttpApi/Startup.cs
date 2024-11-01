using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.GraphQL;
using Giantnodes.Infrastructure.GraphQL.Scalars;
using Giantnodes.Service.Identity.Infrastructure;
using Giantnodes.Service.Supervisor.Components;
using Giantnodes.Service.Supervisor.Domain;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.HttpApi.Types;
using Giantnodes.Service.Supervisor.Persistence;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;

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
            .SetupDomain(_configuration, _environment)
            .SetupPersistence(_configuration, _environment)
            .SetupInfrastructure(_configuration, _environment)
            .SetupComponents(_configuration, _environment);

        services
            .AddGraphQLServer()
            .ModifyOptions(options => options.DefaultFieldBindingFlags = FieldBindingFlags.Default)
            .AddGiantnodesConfiguration()
            .AddGlobalObjectIdentification()
            .AddMutationConventions()
            .AddHttpApiTypes()
            .AddDomainTypes()
            .AddProjections()
            .AddPagingArguments()
            .AddFiltering(options =>
            {
                options.BindRuntimeType<LibraryName, StringOperationFilterInputType>();
                options.BindRuntimeType<LibrarySlug, StringOperationFilterInputType>();

                options.BindRuntimeType<char, CharOperationFilterInputType>();

                options.AddDefaults();
            })
            .AddSorting(options =>
            {
                options.BindRuntimeType<LibraryName, DefaultSortEnumType>();
                options.BindRuntimeType<LibrarySlug, DefaultSortEnumType>();

                options.AddDefaults();
            })
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