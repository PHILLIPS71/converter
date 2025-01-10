using Giantnodes.Infrastructure.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Objects;

[ObjectType<PipelineDefinition>]
public static partial class PipelineDefinitionType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineDefinition> descriptor)
    {
        descriptor
            .Field(f => f.Name);

        descriptor
            .Field(f => f.Description);

        descriptor
            .Field(f => f.Specifications);
    }
}