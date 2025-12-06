using Giantnodes.Infrastructure.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Pipelines;

[ObjectType<PipelineStageDefinition>]
internal static partial class PipelineSpecificationDefinitionType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineStageDefinition> descriptor)
    {
        descriptor
            .Field(f => f.Name);
    }
}
