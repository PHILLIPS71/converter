using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

internal sealed class PipelineOperationFactory : IPipelineOperationFactory
{
    private readonly IEnumerable<IPipelineOperation> _steps;

    public PipelineOperationFactory(IEnumerable<IPipelineOperation> steps)
    {
        _steps = steps;
    }

    public ErrorOr<IPipelineOperation> Create(string name)
    {
        var step = _steps.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (step == null)
            return Error.NotFound(description: $"step '{name}' cannot be found");

        return ErrorOrFactory.From(step);
    }
}
