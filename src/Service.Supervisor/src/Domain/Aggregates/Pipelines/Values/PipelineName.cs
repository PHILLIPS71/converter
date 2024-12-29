using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Validation;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public sealed record PipelineName : ValueObject
{
    private PipelineName(string value) => Value = value;

    public string Value { get; }

    public static ErrorOr<PipelineName> Create(string value)
    {
        var result = NameValidation.ValidateName(value, nameof(Pipeline));
        if (result.IsError)
            return result.Errors;

        return new PipelineName(result.Value);
    }
}