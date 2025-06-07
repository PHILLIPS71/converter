using ErrorOr;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Giantnodes.Infrastructure.Pipelines;

internal sealed class YamlPipelineBuilder : IYamlPipelineBuilder
{
    private readonly ILogger<YamlPipelineBuilder> _logger;

    public YamlPipelineBuilder(ILogger<YamlPipelineBuilder> logger)
    {
        _logger = logger;
    }

    public ErrorOr<PipelineDefinition> Build(string content, CancellationToken cancellation = default)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<PipelineDefinition>(content);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "yaml pipeline definition could not be deserialized. error {Error}", ex.Message);
            return Error.Unexpected(description: "yaml pipeline definition could not be deserialized");
        }
    }
}