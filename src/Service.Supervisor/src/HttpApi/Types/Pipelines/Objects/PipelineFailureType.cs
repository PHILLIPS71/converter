using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Objects;

[ObjectType<PipelineFailure>]
public static partial class PipelineFailureType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineFailure> descriptor)
    {
        descriptor
            .Field(f => f.FailedAt);

        descriptor
            .Field(f => f.Reason);
    }
}
