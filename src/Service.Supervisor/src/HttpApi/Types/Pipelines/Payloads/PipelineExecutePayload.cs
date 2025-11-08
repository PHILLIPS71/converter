using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Payloads;

internal sealed record PipelineExecutePayload
{
    public ICollection<Result> Results { get; init; } = [];

    internal sealed record Result
    {
        public required FileSystemFile File { get; init; }

        public PipelineExecution? Execution { get; init; }

        public IEnumerable<DomainFault.ErrorInfo>? Faults { get; init; }
    }
}

[ObjectType<PipelineExecutePayload>]
public static partial class PipelineExecutePayloadType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineExecutePayload> descriptor)
    {
        descriptor
            .Field(f => f.Results);
    }
}

[ObjectType<PipelineExecutePayload.Result>]
public static partial class PipelineExecutePayloadResultType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineExecutePayload.Result> descriptor)
    {
        descriptor.Name("PipelineExecutePayloadResult");

        descriptor
            .Field(f => f.File);

        descriptor
            .Field(f => f.Execution);

        descriptor
            .Field(f => f.Faults);
    }
}
