using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.GraphQL;
using Giantnodes.Service.Supervisor.Components;
using Giantnodes.Service.Supervisor.Domain;
using Giantnodes.Service.Supervisor.Domain.Values;
using Giantnodes.Service.Supervisor.HttpApi.Endpoints;
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
            .ModifyCostOptions(options => options.EnforceCostLimits = false)
            .ModifyPagingOptions(options => options.IncludeTotalCount = true)
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
                options.BindRuntimeType<Name, DefaultSortEnumType>();
                options.BindRuntimeType<Slug, DefaultSortEnumType>();
                options.AddDefaults();
            })
            .AddConvention<IFilterConvention>(new FilterConventionExtension(options =>
            {
                options.BindRuntimeType<Name, StringOperationFilterInputType>();
                options.BindRuntimeType<Slug, StringOperationFilterInputType>();
            }));
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
