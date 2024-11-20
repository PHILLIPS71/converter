using FluentValidation;

namespace Giantnodes.Service.Runner.Contracts.Probing.Jobs;

public sealed record FileSystemProbe
{
    public sealed record Job
    {
        public required string Path { get; init; }
    }
    
    public sealed class Validator : AbstractValidator<Job>
    {
        public Validator()
        {
            RuleFor(p => p.Path)
                .NotEmpty();
        }
    }
}