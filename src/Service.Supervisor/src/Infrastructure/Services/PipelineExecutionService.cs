using ErrorOr;
using Giantnodes.Infrastructure.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

namespace Giantnodes.Service.Supervisor.Infrastructure.Services;

public sealed class PipelineExecutionService : IPipelineExecutionService
{
    private readonly IConvertPipeline _pipeline;
    private readonly IYamlPipelineBuilder _builder;

    public PipelineExecutionService(IConvertPipeline pipeline, IYamlPipelineBuilder builder)
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

        var execution = pipeline.CreateExecution(definition.Value, file);
        var result = await _pipeline.ExecuteAsync(execution.Definition, execution.Context, cancellation);
        if (result.IsError)
            return result.Errors;

        return execution;
    }
}