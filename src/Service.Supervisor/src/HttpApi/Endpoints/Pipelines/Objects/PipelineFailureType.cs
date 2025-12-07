using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Pipelines;

[ObjectType<PipelineFailure>]
internal static partial class PipelineFailureType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineFailure> descriptor)
    {
        descriptor
            .Field(f => f.FailedAt);

        descriptor
            .Field(f => f.Reason);
    }
}
