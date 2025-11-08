using NSubstitute;
using Xunit;

namespace Giantnodes.Infrastructure.Tests;

public sealed class UnitOfWorkAccessorTests
{
    public sealed class Context
    {
        private readonly UnitOfWorkAccessor _sut = new();

        [Fact]
        public void Should_return_null_when_no_context_is_set()
        {
            // Act
            var result = _sut.Context;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Should_return_set_context()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWorkContext>();

            // Act
            _sut.SetCurrent(context);
            var result = _sut.Context;

            // Assert
            Assert.Same(context, result);
        }

        [Fact]
        public void Should_return_null_after_clearing_context()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWorkContext>();
            _sut.SetCurrent(context);

            // Act
            _sut.SetCurrent(null);
            var result = _sut.Context;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Should_share_context_between_different_accessor_instances()
        {
            // Arrange
            var accessor1 = new UnitOfWorkAccessor();
            var accessor2 = new UnitOfWorkAccessor();
            var context = Substitute.For<IUnitOfWorkContext>();

            // Act
            accessor1.SetCurrent(context);

            // Assert
            // both accessors should see the same context since they use the same static AsyncLocal
            Assert.Same(context, accessor1.Context);
            Assert.Same(context, accessor2.Context);
        }

        [Fact]
        public async Task Should_maintain_context_across_async_operations()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWorkContext>();
            _sut.SetCurrent(context);

            // Act
            var result = await Task.Run(() => _sut.Context);

            // Assert
            Assert.Same(context, result);
        }
    }

    public sealed class SetCurrent
    {
        private readonly UnitOfWorkAccessor _sut = new();

        [Fact]
        public void Should_set_context()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWorkContext>();

            // Act
            _sut.SetCurrent(context);

            // Assert
            Assert.Same(context, _sut.Context);
        }

        [Fact]
        public void Should_accept_null_context()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWorkContext>();
            _sut.SetCurrent(context);

            // Act
            _sut.SetCurrent(null);

            // Assert
            Assert.Null(_sut.Context);
        }

        [Fact]
        public void Should_overwrite_existing_context()
        {
            // Arrange
            var oldContext = Substitute.For<IUnitOfWorkContext>();
            var newContext = Substitute.For<IUnitOfWorkContext>();
            _sut.SetCurrent(oldContext);

            // Act
            _sut.SetCurrent(newContext);

            // Assert
            Assert.Same(newContext, _sut.Context);
        }
    }
}
