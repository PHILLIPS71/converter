using Giantnodes.Service.Supervisor.Contracts.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Inputs;

public class PipelineExecuteType : InputObjectType<PipelineExecute.Command>
{
    protected override void Configure(IInputObjectTypeDescriptor<PipelineExecute.Command> descriptor)
    {
        descriptor
            .Field(f => f.PipelineId)
            .ID();

        descriptor
            .Field(f => f.FileId)
            .ID();
    }
}