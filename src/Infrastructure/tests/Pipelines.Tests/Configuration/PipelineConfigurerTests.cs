using ErrorOr;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Giantnodes.Infrastructure.Pipelines.Tests;

public sealed class PipelineConfigurerTests
{
    public abstract class Fixture
    {
        internal readonly PipelineConfigurer _sut;
        internal readonly ServiceCollection _services;

        protected Fixture()
        {
            _services = [];
            _sut = new PipelineConfigurer(_services);
        }
    }

    public sealed class AddOperation : Fixture
    {
        [Fact]
        public void Should_register_operation_as_interface()
        {
            // Act
            _sut.AddOperation<TestOperation>();

            // Assert
            var provider = _services.BuildServiceProvider();
            var operations = provider.GetServices<IPipelineOperation>().ToList();

            Assert.Single(operations);
            Assert.IsType<TestOperation>(operations[0]);
        }

        [Fact]
        public void Should_register_operation_as_concrete_type()
        {
            // Act
            _sut.AddOperation<TestOperation>();

            // Assert
            var provider = _services.BuildServiceProvider();
            var operation = provider.GetService<TestOperation>();

            Assert.NotNull(operation);
        }

        [Fact]
        public void Should_register_multiple_operations()
        {
            // Act
            _sut.AddOperation<TestOperation>();
            _sut.AddOperation<AnotherTestOperation>();

            // Assert
            var provider = _services.BuildServiceProvider();
            var operations = provider.GetServices<IPipelineOperation>().ToList();

            Assert.Equal(2, operations.Count);
            Assert.Contains(operations, operation => operation is TestOperation);
            Assert.Contains(operations, operation => operation is AnotherTestOperation);
        }

        [Fact]
        public void Should_support_fluent_chaining_multiple_operations()
        {
            // Act
            _sut
                .AddOperation<TestOperation>()
                .AddOperation<AnotherTestOperation>();

            // Assert
            var provider = _services.BuildServiceProvider();
            var operations = provider.GetServices<IPipelineOperation>().ToList();

            Assert.Equal(2, operations.Count);
        }

        [Fact]
        public void Should_not_register_duplicate_operations()
        {
            // Act
            _sut.AddOperation<TestOperation>();
            _sut.AddOperation<TestOperation>();

            // Assert
            var provider = _services.BuildServiceProvider();
            var operations = provider.GetServices<IPipelineOperation>().ToList();

            Assert.Single(operations);
        }

        [Fact]
        public void Should_register_operations_as_transient()
        {
            // Act
            _sut.AddOperation<TestOperation>();

            // Assert
            var provider = _services.BuildServiceProvider();
            var operation1 = provider.GetService<TestOperation>();
            var operation2 = provider.GetService<TestOperation>();

            Assert.NotSame(operation1, operation2);
        }

        private sealed class TestOperation : IPipelineOperation
        {
            public string Name => "test/operation";

            public Task<ErrorOr<IReadOnlyDictionary<string, object>>> ExecuteAsync(
                PipelineStepDefinition definition,
                PipelineContext context,
                CancellationToken cancellation = default)
            {
                return Task.FromResult<ErrorOr<IReadOnlyDictionary<string, object>>>(
                    new Dictionary<string, object>().AsReadOnly());
            }
        }

        private sealed class AnotherTestOperation : IPipelineOperation
        {
            public string Name => "test/another";

            public Task<ErrorOr<IReadOnlyDictionary<string, object>>> ExecuteAsync(
                PipelineStepDefinition definition,
                PipelineContext context,
                CancellationToken cancellation = default)
            {
                return Task.FromResult<ErrorOr<IReadOnlyDictionary<string, object>>>(
                    new Dictionary<string, object>().AsReadOnly());
            }
        }
    }
}
