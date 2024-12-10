using ErrorOr;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

internal abstract class PipelinePublishSpecification<TContext> : IPipelineSpecification<TContext>
    where TContext : PipelineContext
{
    public async Task<ErrorOr<TContext>> ExecuteAsync(
        TContext context,
        CancellationToken cancellation = default)
    {
        var message = CreateMessage(context);

        var endpoint = context.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        await endpoint.Publish(message, cancellation);

        return context;
    }

    public abstract string Name { get; }

    public abstract ErrorOr<Success> Configure(IDictionary<string, object>? inputs);

    protected abstract object CreateMessage(TContext context);
}