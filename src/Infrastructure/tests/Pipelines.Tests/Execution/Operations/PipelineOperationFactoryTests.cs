using ErrorOr;
using NSubstitute;
using Xunit;

namespace Giantnodes.Infrastructure.Pipelines.Tests;

public sealed class PipelineOperationFactoryTests
{
    public abstract class Fixture
    {
        internal readonly PipelineOperationFactory _sut;
        internal readonly List<IPipelineOperation> _operations;

        protected Fixture()
        {
            _operations = [];
            _sut = new PipelineOperationFactory(_operations);
        }

        protected static IPipelineOperation CreateOperation(string name)
        {
            var operation = Substitute.For<IPipelineOperation>();
            operation.Name.Returns(name);
            return operation;
        }
    }

    public sealed class Create : Fixture
    {
        [Fact]
        public void Should_return_operation_when_name_matches()
        {
            // Arrange
            var operation = CreateOperation("giantnodes/probe");
            _operations.Add(operation);

            // Act
            var result = _sut.Create("giantnodes/probe");

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(operation, result.Value);
        }

        [Fact]
        public void Should_return_operation_when_name_matches_case_insensitive()
        {
            // Arrange
            var operation = CreateOperation("giantnodes/probe");
            _operations.Add(operation);

            // Act
            var result = _sut.Create("GIANTNODES/PROBE");

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(operation, result.Value);
        }

        [Fact]
        public void Should_return_error_when_operation_not_found()
        {
            // Arrange
            var operation = CreateOperation("giantnodes/probe");
            _operations.Add(operation);

            // Act
            var result = _sut.Create("giantnodes/encode");

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
            Assert.Equal("step 'giantnodes/encode' cannot be found", result.FirstError.Description);
        }

        [Fact]
        public void Should_return_error_when_no_operations_registered()
        {
            // Arrange
            // No operations added

            // Act
            var result = _sut.Create("giantnodes/probe");

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
        }
    }
}
