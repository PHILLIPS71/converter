using ErrorOr;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

internal sealed class YamlPipelineBuilder : IYamlPipelineBuilder
{
    private readonly IServiceScopeFactory _factory;
    private readonly ILogger<YamlPipelineBuilder> _logger;

    public YamlPipelineBuilder(
        IServiceScopeFactory factory,
        ILogger<YamlPipelineBuilder> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public ErrorOr<PipelineDefinition> Build(string content, CancellationToken cancellation = default)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yaml = deserializer.Deserialize<YamlPipelineDefinition>(content);

            using var scope = _factory.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IPipelineSpecificationFactory>();

            var specifications = new List<IPipelineSpecification>();
            foreach (var job in yaml.Jobs)
            {
                var specification = factory.Create(job.Uses, job.With);
                if (specification.IsError)
                    return specification.Errors;

                specifications.Add(specification.Value);
            }

            var definition = new PipelineDefinition
            {
                Name = yaml.Name,
                Description = yaml.Description,
                Specifications = specifications.AsReadOnly()
            };

            return definition;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "yaml pipeline definition could not be deserialized. error {Exception}", ex.Message);
            return Error.Unexpected(description: "yaml pipeline definition could not be deserialized");
        }
    }

    private sealed record YamlPipelineDefinition
    {
        public required string Name { get; init; }

        public string? Description { get; init; }

        public ICollection<YamlPipelineJobDefinition> Jobs { get; init; } = [];
    }

    private sealed record YamlPipelineJobDefinition
    {
        public required string Name { get; init; }

        public required string Uses { get; init; }

        public string? If { get; init; }

        public IDictionary<string, object>? With { get; init; }
    }
}