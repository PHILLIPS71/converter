namespace Giantnodes.Infrastructure.Pipelines;

internal abstract record PipelineContext
{
    public IServiceProvider ServiceProvider { get; }

    protected PipelineContext(IServiceProvider provider)
    {
        ServiceProvider = provider;
    }
}