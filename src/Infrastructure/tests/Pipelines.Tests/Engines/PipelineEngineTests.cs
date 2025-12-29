using ErrorOr;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Giantnodes.Infrastructure.Pipelines.Tests;

public sealed class PipelineEngineTests
{
    public abstract class Fixture
    {
        internal readonly PipelineEngine _sut;

        internal readonly IPipelineStageEngine _engine;
        internal readonly FakeLogger<PipelineEngine> _logger;

        protected Fixture()
        {
            _engine = Substitute.For<IPipelineStageEngine>();
            _logger = new FakeLogger<PipelineEngine>();

            _sut = new PipelineEngine(_engine, _logger);
        }

        protected static PipelineDefinition CreateDefinition(
            string name,
            IDictionary<string, PipelineStageDefinition> stages)
        {
            return new PipelineDefinition
            {
                Name = name,
                Stages = stages
            };
        }

        protected static PipelineStageDefinition CreateStage(string? id, string name, ICollection<string>? needs = null)
        {
            return new PipelineStageDefinition
            {
                Id = id,
                Name = name,
                Needs = needs ?? []
            };
        }
    }

    public sealed class ExecuteAsync : Fixture
    {
        [Fact]
        public async Task Should_return_success_when_pipeline_has_no_stages()
        {
            // Arrange
            var definition = CreateDefinition("empty-pipeline", new Dictionary<string, PipelineStageDefinition>());
            var context = new PipelineContext(Guid.NewGuid());

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.False(result.IsError);
            await _engine
                .DidNotReceive()
                .ExecuteAsync(Arg.Any<PipelineContext>(), Arg.Any<PipelineStageDefinition>());
        }

        [Fact]
        public async Task Should_return_errors_when_definition_to_graph_fails()
        {
            // Arrange
            var stage1 = CreateStage("stage-1", "Stage 1", ["non-existent"]);
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage1
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("invalid-pipeline", stages);

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
        }

        [Fact]
        public async Task Should_execute_single_stage_successfully()
        {
            // Arrange
            var stage = CreateStage("stage-1", "Stage 1");
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage
                }
            };

            var definition = CreateDefinition("single-stage-pipeline", stages);
            var context = new PipelineContext(Guid.NewGuid());

            _engine
                .ExecuteAsync(context, stage, Arg.Any<CancellationToken>())
                .Returns(Result.Success);

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.False(result.IsError);
            await _engine.Received(1).ExecuteAsync(context, stage, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_execute_independent_stages_in_parallel()
        {
            // Arrange
            var stage1 = CreateStage("stage-1", "Stage 1");
            var stage2 = CreateStage("stage-2", "Stage 2");
            var stage3 = CreateStage("stage-3", "Stage 3");
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage1
                },
                {
                    "stage-2", stage2
                },
                {
                    "stage-3", stage3
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("parallel-pipeline", stages);

            var tcs1 = new TaskCompletionSource<ErrorOr<Success>>();
            var tcs2 = new TaskCompletionSource<ErrorOr<Success>>();
            var tcs3 = new TaskCompletionSource<ErrorOr<Success>>();

            _engine.ExecuteAsync(context, stage1, Arg.Any<CancellationToken>()).Returns(tcs1.Task);
            _engine.ExecuteAsync(context, stage2, Arg.Any<CancellationToken>()).Returns(tcs2.Task);
            _engine.ExecuteAsync(context, stage3, Arg.Any<CancellationToken>()).Returns(tcs3.Task);

            // Act
            var execute = _sut.ExecuteAsync(definition, context);

            // complete stages in reverse order to verify parallel execution
            tcs3.SetResult(Result.Success);
            tcs2.SetResult(Result.Success);
            tcs1.SetResult(Result.Success);

            var result = await execute;

            // Assert
            Assert.False(result.IsError);
            await _engine.Received(1).ExecuteAsync(context, stage1, Arg.Any<CancellationToken>());
            await _engine.Received(1).ExecuteAsync(context, stage2, Arg.Any<CancellationToken>());
            await _engine.Received(1).ExecuteAsync(context, stage3, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_execute_dependent_stages_in_correct_order()
        {
            // Arrange
            var stage1 = CreateStage("stage-1", "Stage 1");
            var stage2 = CreateStage("stage-2", "Stage 2", ["stage-1"]);
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage1
                },
                {
                    "stage-2", stage2
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("sequential-pipeline", stages);

            var order = new List<string>();

            _engine
                .ExecuteAsync(context, stage1, Arg.Any<CancellationToken>())
                .ThenDoAsync(async _ =>
                {
                    order.Add("stage-1");
                    await Task.Delay(10);
                })
                .Returns(Result.Success);

            _engine
                .ExecuteAsync(context, stage2, Arg.Any<CancellationToken>())
                .ThenDo(_ => order.Add("stage-2"))
                .Returns(Result.Success);

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(["stage-1", "stage-2"], order);
        }

        [Fact]
        public async Task Should_respect_maximum_parallelism_limit()
        {
            // Arrange
            // create 5 independent stages to exceed the MaxParallelism limit of 3
            var stage1 = CreateStage("stage-1", "Stage 1");
            var stage2 = CreateStage("stage-2", "Stage 2");
            var stage3 = CreateStage("stage-3", "Stage 3");
            var stage4 = CreateStage("stage-4", "Stage 4");
            var stage5 = CreateStage("stage-5", "Stage 5");
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage1
                },
                {
                    "stage-2", stage2
                },
                {
                    "stage-3", stage3
                },
                {
                    "stage-4", stage4
                },
                {
                    "stage-5", stage5
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("parallelism-test-pipeline", stages);

            var concurrentCount = 0;
            var maxConcurrentCount = 0;
            var locked = new object();

            _engine
                .ExecuteAsync(context, Arg.Any<PipelineStageDefinition>(), Arg.Any<CancellationToken>())
                .Returns(_ => Task.FromResult(CreateDelayedResult()));

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.False(result.IsError);
            Assert.True(maxConcurrentCount <= 3, $"max concurrent count was {maxConcurrentCount}, expected <= 3");
            return;

            ErrorOr<Success> CreateDelayedResult()
            {
                lock (locked)
                {
                    concurrentCount++;
                    maxConcurrentCount = Math.Max(maxConcurrentCount, concurrentCount);
                }

                Task.Delay(50).Wait();

                lock (locked)
                    concurrentCount--;

                return Result.Success;
            }
        }

        [Fact]
        public async Task Should_return_error_when_stage_execution_fails()
        {
            // Arrange
            var stage = CreateStage("stage-1", "Stage 1");
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("failing-pipeline", stages);

            var error = Error.Failure(description: "stage execution failed");

            _engine
                .ExecuteAsync(context, stage, Arg.Any<CancellationToken>())
                .Returns(error);

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal("stage execution failed", result.Errors[0].Description);
        }

        [Fact]
        public async Task Should_stop_execution_when_stage_fails()
        {
            // Arrange
            var stage1 = CreateStage("stage-1", "Stage 1");
            var stage2 = CreateStage("stage-2", "Stage 2", ["stage-1"]);
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage1
                },
                {
                    "stage-2", stage2
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("stop-on-failure-pipeline", stages);

            var error = Error.Failure(description: "stage-1 failed");

            _engine
                .ExecuteAsync(context, stage1, Arg.Any<CancellationToken>())
                .Returns(error);

            _engine
                .ExecuteAsync(context, stage2, Arg.Any<CancellationToken>())
                .Returns(Result.Success);

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.True(result.IsError);
            await _engine.Received(1).ExecuteAsync(context, stage1, Arg.Any<CancellationToken>());
            await _engine.DidNotReceive().ExecuteAsync(context, stage2, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_handle_cancellation_gracefully()
        {
            // Arrange
            var stage = CreateStage("stage-1", "Stage 1");
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("cancellation-pipeline", stages);

            var cts = new CancellationTokenSource();
            var tcs = new TaskCompletionSource<ErrorOr<Success>>();

            // link cancellation to the task completion source
            cts.Token.Register(() => tcs.TrySetCanceled());

            _engine
                .ExecuteAsync(context, stage, Arg.Any<CancellationToken>())
                .Returns(tcs.Task);

            // Act
            var execute = _sut.ExecuteAsync(definition, context, cts.Token);
            await cts.CancelAsync();

            // Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => execute);
        }

        [Fact]
        public async Task Should_handle_unexpected_exceptions_and_return_error()
        {
            // Arrange
            var stage = CreateStage("stage-1", "Stage 1");
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("exception-pipeline", stages);

            var exception = new InvalidOperationException("unexpected error");

            _engine
                .ExecuteAsync(context, stage, Arg.Any<CancellationToken>())
                .Throws(exception);

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Unexpected, result.FirstError.Type);
        }

        [Fact]
        public async Task Should_return_first_error_when_multiple_parallel_stages_fail()
        {
            // Arrange
            var stage1 = CreateStage("stage-1", "Stage 1");
            var stage2 = CreateStage("stage-2", "Stage 2");
            var stage3 = CreateStage("stage-3", "Stage 3");
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage1
                },
                {
                    "stage-2", stage2
                },
                {
                    "stage-3", stage3
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("multiple-failures-pipeline", stages);

            var error1 = Error.Failure(description: "stage-1 failed");
            var error2 = Error.Validation(description: "stage-2 validation failed");
            var error3 = Error.Conflict(description: "stage-3 conflict");

            _engine.ExecuteAsync(context, stage1, Arg.Any<CancellationToken>()).Returns(error1);
            _engine.ExecuteAsync(context, stage2, Arg.Any<CancellationToken>()).Returns(error2);
            _engine.ExecuteAsync(context, stage3, Arg.Any<CancellationToken>()).Returns(error3);

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);

            // verify it's one of the expected errors (whichever stage completed first)
            var error = new[] { "stage-1 failed", "stage-2 validation failed", "stage-3 conflict" };
            Assert.Contains(result.Errors[0].Description, error);
        }

        [Fact]
        public async Task Should_execute_stage_after_all_dependencies_complete()
        {
            // Arrange
            var stage1 = CreateStage("stage-1", "Stage 1");
            var stage2 = CreateStage("stage-2", "Stage 2");
            var stage3 = CreateStage("stage-3", "Stage 3", ["stage-1", "stage-2"]);
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage1
                },
                {
                    "stage-2", stage2
                },
                {
                    "stage-3", stage3
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("dependency-tracking-pipeline", stages);

            var order = new List<string>();

            _engine
                .ExecuteAsync(context, stage1, Arg.Any<CancellationToken>())
                .ThenDoAsync(async _ =>
                {
                    order.Add("stage-1");
                    await Task.Delay(10);
                })
                .Returns(Result.Success);

            _engine
                .ExecuteAsync(context, stage2, Arg.Any<CancellationToken>())
                .ThenDoAsync(async _ =>
                {
                    order.Add("stage-2");
                    await Task.Delay(10);
                })
                .Returns(Result.Success);

            _engine
                .ExecuteAsync(context, stage3, Arg.Any<CancellationToken>())
                .ThenDo(_ => order.Add("stage-3"))
                .Returns(Result.Success);

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(3, order.Count);

            // stage-3 must execute last after both dependencies complete
            var stage1Index = order.IndexOf("stage-1");
            var stage2Index = order.IndexOf("stage-2");
            var stage3Index = order.IndexOf("stage-3");
            Assert.True(stage1Index < stage3Index);
            Assert.True(stage2Index < stage3Index);
            Assert.Equal(2, stage3Index);
        }

        [Fact]
        public async Task Should_execute_complex_dependency_graph_correctly()
        {
            // Arrange
            // create a diamond dependency graph:
            //     stage1
            //     /    \
            //  stage2  stage3
            //     \    /
            //     stage4
            var stage1 = CreateStage("stage-1", "Stage 1");
            var stage2 = CreateStage("stage-2", "Stage 2", ["stage-1"]);
            var stage3 = CreateStage("stage-3", "Stage 3", ["stage-1"]);
            var stage4 = CreateStage("stage-4", "Stage 4", ["stage-2", "stage-3"]);
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                {
                    "stage-1", stage1
                },
                {
                    "stage-2", stage2
                },
                {
                    "stage-3", stage3
                },
                {
                    "stage-4", stage4
                }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("complex-pipeline", stages);

            var order = new List<string>();
            var locked = new object();

            _engine
                .ExecuteAsync(context, stage1, Arg.Any<CancellationToken>())
                .ThenDoAsync(async _ =>
                {
                    lock (locked)
                        order.Add("stage-1");

                    await Task.Delay(10);
                })
                .Returns(Result.Success);

            _engine
                .ExecuteAsync(context, stage2, Arg.Any<CancellationToken>())
                .ThenDoAsync(async _ =>
                {
                    lock (locked)
                        order.Add("stage-2");

                    await Task.Delay(10);
                })
                .Returns(Result.Success);

            _engine
                .ExecuteAsync(context, stage3, Arg.Any<CancellationToken>())
                .ThenDoAsync(async _ =>
                {
                    lock (locked)
                        order.Add("stage-3");

                    await Task.Delay(10);
                })
                .Returns(Result.Success);

            _engine
                .ExecuteAsync(context, stage4, Arg.Any<CancellationToken>())
                .ThenDo(_ =>
                {
                    lock (locked)
                        order.Add("stage-4");
                })
                .Returns(Result.Success);

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(4, order.Count);

            // stage 1 must execute first
            Assert.Equal("stage-1", order[0]);

            // stage 2 and stage 3 must execute after stage 1 but before stage4
            var stage2Index = order.IndexOf("stage-2");
            var stage3Index = order.IndexOf("stage-3");
            var stage4Index = order.IndexOf("stage-4");
            Assert.True(stage2Index < stage4Index);
            Assert.True(stage3Index < stage4Index);

            // stage4 must execute last
            Assert.Equal("stage-4", order[3]);
        }

        [Fact]
        public async Task Should_await_all_running_tasks_before_returning_on_stage_failure()
        {
            // Arrange
            var stage1 = CreateStage("stage-1", "Stage 1");
            var stage2 = CreateStage("stage-2", "Stage 2");
            var stage3 = CreateStage("stage-3", "Stage 3");
            var stages = new Dictionary<string, PipelineStageDefinition>
            {
                { "stage-1", stage1 },
                { "stage-2", stage2 },
                { "stage-3", stage3 }
            };

            var context = new PipelineContext(Guid.NewGuid());
            var definition = CreateDefinition("cleanup-test-pipeline", stages);

            var tcs2 = new TaskCompletionSource<ErrorOr<Success>>();
            var tcs3 = new TaskCompletionSource<ErrorOr<Success>>();

            var stage2Completed = false;
            var stage3Completed = false;

            // stage 1 fails immediately
            _engine
                .ExecuteAsync(context, stage1, Arg.Any<CancellationToken>())
                .Returns(Error.Failure(description: "stage-1 failed"));

            // stage 2 simulates work and tracks completion
            _engine
                .ExecuteAsync(context, stage2, Arg.Any<CancellationToken>())
                .Returns(tcs2.Task);

            // stage 3 simulates longer work and tracks completion
            _engine
                .ExecuteAsync(context, stage3, Arg.Any<CancellationToken>())
                .Returns(tcs3.Task);

            // simulate async work completion for stages 2 and 3
            _ = Task.Run(async () =>
            {
                await Task.Delay(100);
                stage2Completed = true;
                tcs2.SetResult(Result.Success);
            });

            _ = Task.Run(async () =>
            {
                await Task.Delay(150);
                stage3Completed = true;
                tcs3.SetResult(Result.Success);
            });

            // Act
            var result = await _sut.ExecuteAsync(definition, context);

            // Assert
            Assert.True(result.IsError);
            Assert.True(stage2Completed, "stage 2 should have completed before ExecuteAsync returned");
            Assert.True(stage3Completed, "stage 3 should have completed before ExecuteAsync returned");
        }
    }
}
