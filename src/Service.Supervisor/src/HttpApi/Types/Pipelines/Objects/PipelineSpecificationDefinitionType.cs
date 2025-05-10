using Giantnodes.Infrastructure.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Objects;

[ObjectType<PipelineStageDefinition>]
public static partial class PipelineSpecificationDefinitionType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineStageDefinition> descriptor)
    {
        descriptor
            .Field(f => f.Name);
    }
}