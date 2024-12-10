using ErrorOr;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

internal sealed class SubtitleStripSpecification : PipelinePublishSpecification<VideoPipeline.Context>
{
    public override string Name => "actions/checkout@v4";

    public override ErrorOr<Success> Configure(IDictionary<string, object>? inputs)
    {
        return Result.Success;
    }

    protected override object CreateMessage(VideoPipeline.Context context)
    {
        throw new NotImplementedException();
    }
}