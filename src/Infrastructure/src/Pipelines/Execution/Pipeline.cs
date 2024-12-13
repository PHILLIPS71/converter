using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.Pipelines;

public abstract class Pipeline<TInput, TResult> : IPipeline<TInput, TResult>
{
    private readonly IPipelineSpecificationFactory _factory;
    private readonly ILogger<Pipeline<TInput, TResult>> _logger;

    protected Pipeline(IPipelineSpecificationFactory factory, ILogger<Pipeline<TInput, TResult>> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<ErrorOr<TResult>> ExecuteAsync(
        PipelineDefinition definition,
        TInput input,
        CancellationToken cancellation = default)
    {
        foreach (var specification in definition.Specifications)
        {
            cancellation.ThrowIfCancellationRequested();

            try
            {
                var executable = _factory.Create(specification.Uses);
                if (executable.IsError)
                    return executable.Errors;

                var result = await executable.Value.ExecuteAsync(specification, cancellation);
                if (result.IsError)
                    return result.Errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an unexpected error occurred executing pipeline. error: {Error}", ex.Message);
                return Error.Unexpected(description: $"an unexpected error occurred executing pipeline. error: {ex.Message}");
            }
        }

        return CreateResult();
    }

    protected abstract TResult CreateResult();
}