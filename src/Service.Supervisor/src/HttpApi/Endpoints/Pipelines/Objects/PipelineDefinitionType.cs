using Giantnodes.Infrastructure.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Pipelines;

[ObjectType<PipelineDefinition>]
internal static partial class PipelineDefinitionType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineDefinition> descriptor)
    {
        descriptor
            .Field(f => f.Name);

        descriptor
            .Field(f => f.Description);
    }
}
