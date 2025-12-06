using Giantnodes.Service.Supervisor.Contracts.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Pipelines;

internal sealed class PipelineUpdateType : InputObjectType<PipelineUpdate.Command>
{
    protected override void Configure(IInputObjectTypeDescriptor<PipelineUpdate.Command> descriptor)
    {
        descriptor
            .Field(f => f.Id)
            .ID();
    }
}
