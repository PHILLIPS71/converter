using Xunit;

namespace Giantnodes.Infrastructure.Tests;

public sealed class UnitOfWorkFailedEventArgsTests
{
    public sealed class Constructor
    {
        [Fact]
        public void Should_set_exception_property()
        {
            // Arrange
            var exception = new InvalidOperationException("test exception");

            // Act
            var args = new UnitOfWorkFailedEventArgs(exception);

            // Assert
            Assert.Same(exception, args.Exception);
        }

        [Fact]
        public void Should_throw_argument_null_exception_exception_is_null()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UnitOfWorkFailedEventArgs(null!));
        }

        [Fact]
        public void Should_inherit_from_event_args()
        {
            // Arrange
            var exception = new InvalidOperationException();

            // Act
            var args = new UnitOfWorkFailedEventArgs(exception);

            // Assert
            Assert.IsType<EventArgs>(args, exactMatch: false);
        }

        [Theory]
        [InlineData(typeof(ArgumentException))]
        [InlineData(typeof(InvalidOperationException))]
        [InlineData(typeof(NotSupportedException))]
        [InlineData(typeof(TimeoutException))]
        public void Should_handle_different_exception_types(Type type)
        {
            // Arrange
            var exception = (Exception)Activator.CreateInstance(type, "test message")!;

            // Act
            var args = new UnitOfWorkFailedEventArgs(exception);

            // Assert
            Assert.Same(exception, args.Exception);
            Assert.IsType(type, args.Exception);
        }
    }
}
