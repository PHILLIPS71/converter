using ErrorOr;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Giantnodes.Infrastructure.Pipelines;

internal sealed class YamlPipelineBuilder : IYamlPipelineBuilder
{
    private readonly ILogger<YamlPipelineBuilder> _logger;
    private readonly PipelineDefinition.Validator _validator;

    public YamlPipelineBuilder(ILogger<YamlPipelineBuilder> logger)
    {
        _logger = logger;
        _validator = new PipelineDefinition.Validator();
    }

    public ErrorOr<PipelineDefinition> Build(string content, CancellationToken cancellation = default)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Error.Validation(description: "yaml pipeline content is empty");

        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var pipeline = deserializer.Deserialize<PipelineDefinition>(content);

            var validation = _validator.Validate(pipeline);
            if (!validation.IsValid)
            {
                var errors = validation
                    .Errors
                    .ConvertAll(e => Error.Validation(description: e.ErrorMessage));

                _logger.LogWarning("yaml pipeline definition validation failed with {Count} error(s)", errors.Count);
                return errors;
            }

            return pipeline;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("yaml pipeline builder was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "yaml pipeline definition could not be deserialized. error {Error}", ex.Message);
            return Error.Unexpected(description: "yaml pipeline definition could not be deserialized");
        }
    }
}
