using ErrorOr;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

internal sealed class PipelineSpecificationFactory : IPipelineSpecificationFactory
{
    private readonly IServiceScopeFactory _factory;
    private readonly Lazy<Dictionary<string, Type>> _specifications;

    public PipelineSpecificationFactory(IServiceScopeFactory factory)
    {
        _factory = factory;
        _specifications = new Lazy<Dictionary<string, Type>>(GetPipelineSpecifications);
    }

    private static Dictionary<string, Type> GetPipelineSpecifications()
    {
        var specifications = typeof(IPipelineSpecification)
            .Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false } && typeof(IPipelineSpecification).IsAssignableFrom(type))
            .ToDictionary(
                type => ((IPipelineSpecification)Activator.CreateInstance(type)!).Name,
                type => type,
                StringComparer.InvariantCultureIgnoreCase
            );

        return specifications;
    }

    public ErrorOr<IPipelineSpecification> Create(string name, IDictionary<string, object>? inputs = null)
    {
        if (!_specifications.Value.TryGetValue(name, out var type))
            return Error.NotFound(description: $"specification '{name}' cannot be found");

        using var scope = _factory.CreateScope();
        var instance = ActivatorUtilities.CreateInstance(scope.ServiceProvider, type);

        if (instance is not IPipelineSpecification specification)
            return Error.Failure(
                description:
                $"type '{instance.GetType().Name}' created for specification '{name}' is not a valid specification implementation");

        var result = specification.Configure(inputs);
        if (result.IsError)
            return result.Errors;

        return ErrorOrFactory.From(specification);
    }
}