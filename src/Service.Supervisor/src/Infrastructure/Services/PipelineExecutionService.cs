using ErrorOr;
using Giantnodes.Infrastructure.Pipelines;
using Giantnodes.Infrastructure.Pipelines.MassTransit;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

namespace Giantnodes.Service.Supervisor.Infrastructure.Services;

public sealed class PipelineExecutionService : IPipelineExecutionService
{
    private readonly MassTransitPipeline _pipeline;
    private readonly IYamlPipelineBuilder _builder;

    public PipelineExecutionService(MassTransitPipeline pipeline, IYamlPipelineBuilder builder)
    {
        _pipeline = pipeline;
        _builder = builder;
    }

    public async Task<ErrorOr<PipelineExecution>> ExecuteAsync(
        Pipeline pipeline,
        FileSystemFile file,
        CancellationToken cancellation = default)
    {
        var definition = _builder.Build(pipeline.Definition, cancellation);
        if (definition.IsError)
            return definition.Errors;

        var execution = pipeline.CreateExecution(file, pipeline.Definition);
        var context = new PipelineContext(new Dictionary<string, object>
        {
            { "pipeline_id", pipeline.Id.ToString() },
            { "pipeline_execution_id", execution.Id.ToString() },
            { "path", file.PathInfo.FullName }
        });

        var result = await _pipeline.ExecuteAsync(definition.Value, context, cancellation);
        if (result.IsError)
            return result.Errors;

        return execution;
    }
}