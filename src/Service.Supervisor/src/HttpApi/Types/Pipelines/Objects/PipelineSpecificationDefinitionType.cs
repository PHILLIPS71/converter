using Giantnodes.Infrastructure.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Objects;

[ObjectType<PipelineSpecificationDefinition>]
public static partial class PipelineSpecificationDefinitionType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineSpecificationDefinition> descriptor)
    {
        descriptor
            .Field(f => f.Name);

        descriptor
            .Field(f => f.Uses);
    }
}