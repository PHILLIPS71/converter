using ErrorOr;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

internal abstract class Pipeline<TInput, TResult> : IPipeline<TInput, TResult>
{
    private readonly IServiceScopeFactory _factory;
    private readonly ILogger<Pipeline<TInput, TResult>> _logger;

    protected Pipeline(IServiceScopeFactory factory, ILogger<Pipeline<TInput, TResult>> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public sealed record Context : PipelineContext
    {
        public Context(IServiceProvider provider)
            : base(provider)
        {
        }
    }

    public async Task<ErrorOr<TResult>> ExecuteAsync(
        PipelineDefinition definition,
        TInput input,
        CancellationToken cancellation = default)
    {
        using var scope = _factory.CreateScope();

        ErrorOr<Context> context = new Context(scope.ServiceProvider);
        foreach (var specification in definition.Specifications.OfType<IPipelineSpecification<Context>>())
        {
            cancellation.ThrowIfCancellationRequested();

            try
            {
                context = await specification.ExecuteAsync(context.Value, cancellation);

                if (context.IsError)
                    return context.Errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an unexpected error occurred executing pipeline. error: {Error}", ex.Message);
                return Error.Unexpected(description: $"an unexpected error occurred executing pipeline. error: {ex.Message}");
            }
        }

        if (context.IsError)
            return context.Errors;

        return CreateResult(context.Value);
    }

    protected abstract TResult CreateResult(Context context);
}