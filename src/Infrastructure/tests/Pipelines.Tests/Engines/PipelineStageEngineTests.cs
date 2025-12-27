using ErrorOr;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using Xunit;

namespace Giantnodes.Infrastructure.Pipelines.Tests;

public sealed class PipelineStageEngineTests
{
    public abstract class Fixture
    {
        internal readonly PipelineStageEngine _sut;

        internal readonly IPipelineOperationFactory _factory;
        internal readonly FakeLogger<PipelineStageEngine> _logger;

        protected Fixture()
        {
            _factory = Substitute.For<IPipelineOperationFactory>();
            _logger = new FakeLogger<PipelineStageEngine>();

            _sut = new PipelineStageEngine(_factory, _logger);
        }

        protected static PipelineStageDefinition CreateStage(
            string id,
            string name,
            params PipelineStepDefinition[] steps)
        {
            return new PipelineStageDefinition
            {
                Id = id,
                Name = name,
                Steps = steps.ToList()
            };
        }

        protected static PipelineStepDefinition CreateStep(string id, string name, string uses)
        {
            return new PipelineStepDefinition
            {
                Id = id,
                Name = name,
                Uses = uses
            };
        }
    }

    public sealed class ExecuteAsync : Fixture
    {
        [Fact]
        public async Task Should_return_success_when_stage_has_no_steps()
        {
            // Arrange
            var stage = CreateStage("stage-1", "Empty Stage");
            var context = new PipelineContext();

            // Act
            var result = await _sut.ExecuteAsync(context, stage);

            // Assert
            Assert.False(result.IsError);
            _factory.DidNotReceive().Create(Arg.Any<string>());
        }

        [Fact]
        public async Task Should_execute_single_step_successfully()
        {
            // Arrange
            var step = CreateStep("step-1", "Step 1", "test-operation");
            var stage = CreateStage("stage-1", "Stage 1", step);
            var context = new PipelineContext();

            var operation = new TestPipelineOperation("test-operation")
            {
                Outputs = new Dictionary<string, object>
                {
                    {
                        "result", "success"
                    }
                }
            };

            _factory.Create("test-operation").Returns(operation);

            // Act
            var result = await _sut.ExecuteAsync(context, stage);

            // Assert
            _factory.Received(1).Create("test-operation");
            Assert.False(result.IsError);
            Assert.Equal("success", context.Outputs["step-1"]["result"]);
        }

        [Fact]
        public async Task Should_return_error_when_factory_fails_to_create_operation()
        {
            // Arrange
            var step = CreateStep("step-1", "Step 1", "unknown-operation");
            var stage = CreateStage("stage-1", "Stage 1", step);
            var context = new PipelineContext();

            _factory
                .Create("unknown-operation")
                .Returns(Error.NotFound(description: "operation 'unknown-operation' not found"));

            // Act
            var result = await _sut.ExecuteAsync(context, stage);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
        }

        [Fact]
        public async Task Should_return_error_when_step_execution_fails()
        {
            // Arrange
            var step = CreateStep("step-1", "Step 1", "test-operation");
            var stage = CreateStage("stage-1", "Stage 1", step);
            var context = new PipelineContext();

            var operation = new TestPipelineOperation("test-operation")
            {
                Error = Error.Failure(description: "step execution failed")
            };

            _factory.Create("test-operation").Returns(operation);

            // Act
            var result = await _sut.ExecuteAsync(context, stage);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Failure, result.FirstError.Type);
        }

        [Fact]
        public async Task Should_stop_execution_at_first_failed_step()
        {
            // Arrange
            var step1 = CreateStep("step-1", "Step 1", "operation-1");
            var step2 = CreateStep("step-2", "Step 2", "operation-2");
            var step3 = CreateStep("step-3", "Step 3", "operation-3");
            var stage = CreateStage("stage-1", "Stage 1", step1, step2, step3);
            var context = new PipelineContext();

            var operation1 = new TestPipelineOperation("operation-1");
            var operation2 = new TestPipelineOperation("operation-2")
            {
                Error = Error.Failure(description: "step-2 failed")
            };
            var operation3 = new TestPipelineOperation("operation-3");

            _factory.Create("operation-1").Returns(operation1);
            _factory.Create("operation-2").Returns(operation2);
            _factory.Create("operation-3").Returns(operation3);

            // Act
            var result = await _sut.ExecuteAsync(context, stage);

            // Assert
            Assert.True(result.IsError);
            _factory.Received(1).Create("operation-1");
            _factory.Received(1).Create("operation-2");
            _factory.DidNotReceive().Create("operation-3");
        }

        [Fact]
        public async Task Should_handle_cancellation_gracefully()
        {
            // Arrange
            var step = CreateStep("step-1", "Step 1", "test-operation");
            var stage = CreateStage("stage-1", "Stage 1", step);
            var context = new PipelineContext();

            var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _sut.ExecuteAsync(context, stage, cts.Token));
        }

        [Fact]
        public async Task Should_return_error_for_unexpected_exceptions()
        {
            // Arrange
            var step = CreateStep("step-1", "Step 1", "test-operation");
            var stage = CreateStage("stage-1", "Stage 1", step);
            var context = new PipelineContext();

            var operation = new TestPipelineOperation("test-operation")
            {
                Exception = new InvalidOperationException("unexpected error")
            };

            _factory.Create("test-operation").Returns(operation);

            // Act
            var result = await _sut.ExecuteAsync(context, stage);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Unexpected, result.FirstError.Type);
        }

        [Fact]
        public async Task Should_store_step_outputs_in_context()
        {
            // Arrange
            var step1 = CreateStep("step-1", "Step 1", "operation-1");
            var step2 = CreateStep("step-2", "Step 2", "operation-2");
            var stage = CreateStage("stage-1", "Stage 1", step1, step2);
            var context = new PipelineContext();

            var operation1 = new TestPipelineOperation("operation-1")
            {
                Outputs = new Dictionary<string, object>
                {
                    {
                        "result", "value1"
                    }
                }
            };
            var operation2 = new TestPipelineOperation("operation-2")
            {
                Outputs = new Dictionary<string, object>
                {
                    {
                        "result", "value2"
                    }
                }
            };

            _factory.Create("operation-1").Returns(operation1);
            _factory.Create("operation-2").Returns(operation2);

            // Act
            var result = await _sut.ExecuteAsync(context, stage);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(2, context.Outputs.Count);
            Assert.Equal("value1", context.Outputs["step-1"]["result"]);
            Assert.Equal("value2", context.Outputs["step-2"]["result"]);
        }
    }

    private sealed class TestPipelineOperation : IPipelineOperation
    {
        public string Name { get; }

        public IReadOnlyDictionary<string, object>? Outputs { get; init; }
        public Error? Error { get; init; }
        public Exception? Exception { get; init; }

        public TestPipelineOperation(string name)
        {
            Name = name;
        }

        public Task<ErrorOr<IReadOnlyDictionary<string, object>>> ExecuteAsync(
            PipelineStepDefinition definition,
            PipelineContext context,
            CancellationToken cancellation = default)
        {
            if (Exception != null)
                throw Exception;

            if (Error.HasValue)
                return Task.FromResult<ErrorOr<IReadOnlyDictionary<string, object>>>(Error.Value);

            var outputs = Outputs ?? new Dictionary<string, object>().AsReadOnly();
            return Task.FromResult(ErrorOrFactory.From(outputs));
        }
    }
}
