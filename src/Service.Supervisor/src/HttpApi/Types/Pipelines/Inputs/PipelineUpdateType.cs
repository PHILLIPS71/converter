using Giantnodes.Service.Supervisor.Contracts.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Inputs;

public class PipelineUpdateType : InputObjectType<PipelineUpdate.Command>
{
    protected override void Configure(IInputObjectTypeDescriptor<PipelineUpdate.Command> descriptor)
    {
        descriptor
            .Field(f => f.Id)
            .ID();
    }
}
