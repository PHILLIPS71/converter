using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.GraphQL;
using Giantnodes.Service.Supervisor.Components;
using Giantnodes.Service.Supervisor.Domain;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.HttpApi.Types;
using Giantnodes.Service.Supervisor.Infrastructure;
using Giantnodes.Service.Supervisor.Persistence;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;

namespace Giantnodes.Service.Supervisor.HttpApi;

internal sealed class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
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
                    const string section = "Cors:AllowedOrigins";

                    var origins = _configuration.GetSection(section).Get<string[]>();
                    if (origins == null || origins.Length == 0)
                        throw new ConfigurationException(section);

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
            .ModifyCostOptions(configure => configure.EnforceCostLimits = false)
            .AddPlatformConfiguration()
            .AddGlobalObjectIdentification()
            .AddMutationConventions()
            .AddQueryContext()
            .AddHttpApiTypes()
            .AddDomainTypes()
            .AddPagingArguments()
            .AddFiltering()
            .AddSorting(options =>
            {
                options.BindRuntimeType<LibraryName, DefaultSortEnumType>();
                options.BindRuntimeType<PipelineName, DefaultSortEnumType>();

                options.BindRuntimeType<LibrarySlug, DefaultSortEnumType>();
                options.BindRuntimeType<PipelineSlug, DefaultSortEnumType>();

                options.AddDefaults();
            })
            .AddConvention<IFilterConvention>(new FilterConventionExtension(options =>
            {
                options.BindRuntimeType<LibraryName, StringOperationFilterInputType>();
                options.BindRuntimeType<PipelineName, StringOperationFilterInputType>();

                options.BindRuntimeType<LibrarySlug, StringOperationFilterInputType>();
                options.BindRuntimeType<PipelineSlug, StringOperationFilterInputType>();
            }))
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
