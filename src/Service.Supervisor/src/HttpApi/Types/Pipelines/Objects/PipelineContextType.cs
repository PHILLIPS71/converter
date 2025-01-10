using Giantnodes.Infrastructure.Pipelines;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Objects;

[ObjectType<PipelineContext>]
public static partial class PipelineContextType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineContext> descriptor)
    {
        descriptor
            .Field(f => f.State)
            .Type<PipelineContextStateEntryType>();
    }
}

internal sealed class PipelineContextStateEntryType : ObjectType<KeyValuePair<string, object>>
{
    protected override void Configure(IObjectTypeDescriptor<KeyValuePair<string, object>> descriptor)
    {
        descriptor.Name("PipelineContextStateEntry");

        descriptor
            .Field(f => f.Key);

        descriptor
            .Field(f => f.Value)
            .Type<NonNullType<StringType>>();
    }
}