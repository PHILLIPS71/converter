using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure.Pipelines;

internal sealed class PipelineConfigurer : IPipelineConfigurer
{
    public IServiceCollection Services { get; }

    public PipelineConfigurer(IServiceCollection services)
    {
        Services = services;
    }

    /// <inheritdoc />
    public IPipelineConfigurer AddOperation<TOperation>()
        where TOperation : IPipelineOperation
    {
        Services.TryAddTransient(typeof(IPipelineOperation), typeof(TOperation));
        Services.TryAddTransient(typeof(TOperation));

        return this;
    }
}
