using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

internal sealed class VideoPipeline : Pipeline<string, string>, IVideoPipeline
{
    public VideoPipeline(IServiceScopeFactory factory, ILogger<VideoPipeline> logger)
        : base(factory, logger)
    {
    }

    protected override string CreateResult(Context context) => string.Empty;
}