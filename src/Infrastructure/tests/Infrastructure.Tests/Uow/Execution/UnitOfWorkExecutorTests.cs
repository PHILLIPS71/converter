using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Giantnodes.Infrastructure.Tests;

public abstract class UnitOfWorkExecutorFixture
{
    internal readonly UnitOfWorkExecutor Sut;

    protected readonly IUnitOfWorkInterceptor Interceptor1;
    protected readonly IUnitOfWorkInterceptor Interceptor2;
    protected readonly UnitOfWork UnitOfWork;

    protected UnitOfWorkExecutorFixture()
    {
        Interceptor1 = Substitute.For<IUnitOfWorkInterceptor>();
        Interceptor2 = Substitute.For<IUnitOfWorkInterceptor>();
        UnitOfWork = Substitute.For<UnitOfWork>(Substitute.For<IUnitOfWorkExecutor>());

        var interceptors = new[] { Interceptor1, Interceptor2 };
        Sut = new UnitOfWorkExecutor(interceptors);
    }
}

public sealed class UnitOfWorkExecutorTests : UnitOfWorkExecutorFixture
{
    public sealed class Constructor : UnitOfWorkExecutorFixture
    {
        [Fact]
        public void Should_accept_empty_interceptor_collection()
        {
            // Arrange
            var interceptors = Array.Empty<IUnitOfWorkInterceptor>();

            // Act & Assert
            var executor = new UnitOfWorkExecutor(interceptors);
            Assert.NotNull(executor);
        }
    }

    public sealed class OnAfterBeginAsync : UnitOfWorkExecutorFixture
    {
        [Fact]
        public async Task Should_execute_all_interceptors()
        {
            // Arrange
            var cancellation = CancellationToken.None;

            // Act
            await Sut.OnAfterBeginAsync(UnitOfWork, cancellation);

            // Assert
            await Interceptor1.Received(1).OnAfterBeginAsync(UnitOfWork, cancellation);
            await Interceptor2.Received(1).OnAfterBeginAsync(UnitOfWork, cancellation);
        }

        [Fact]
        public async Task Should_complete_immediately_when_no_interceptors()
        {
            // Arrange
            var interceptors = Array.Empty<IUnitOfWorkInterceptor>();
            var executor = new UnitOfWorkExecutor(interceptors);

            // Act
            var task = executor.OnAfterBeginAsync(UnitOfWork);

            // Assert
            Assert.True(task.IsCompletedSuccessfully);
            await task;
        }

        [Fact]
        public async Task Should_execute_interceptors_concurrently()
        {
            // Arrange
            var tcs1 = new TaskCompletionSource();
            var tcs2 = new TaskCompletionSource();

            Interceptor1.OnAfterBeginAsync(UnitOfWork, Arg.Any<CancellationToken>()).Returns(tcs1.Task);
            Interceptor2.OnAfterBeginAsync(UnitOfWork, Arg.Any<CancellationToken>()).Returns(tcs2.Task);

            // Act
            var task = Sut.OnAfterBeginAsync(UnitOfWork);

            // complete interceptors in reverse order to verify concurrency
            tcs2.SetResult();
            tcs1.SetResult();

            await task;

            // Assert
            await Interceptor1.Received(1).OnAfterBeginAsync(UnitOfWork, Arg.Any<CancellationToken>());
            await Interceptor2.Received(1).OnAfterBeginAsync(UnitOfWork, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_propagate_exceptions_from_interceptors()
        {
            // Arrange
            var exception = new InvalidOperationException("interceptor failed");
            Interceptor1.OnAfterBeginAsync(UnitOfWork, Arg.Any<CancellationToken>()).Throws(exception);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.OnAfterBeginAsync(UnitOfWork));

            Assert.Same(exception, actual);
        }
    }

    public sealed class OnAfterCommitAsync : UnitOfWorkExecutorFixture
    {
        [Fact]
        public async Task Should_execute_all_interceptors()
        {
            // Arrange
            var cancellation = CancellationToken.None;

            // Act
            await Sut.OnAfterCommitAsync(UnitOfWork, cancellation);

            // Assert
            await Interceptor1.Received(1).OnAfterCommitAsync(UnitOfWork, cancellation);
            await Interceptor2.Received(1).OnAfterCommitAsync(UnitOfWork, cancellation);
        }

        [Fact]
        public async Task Should_complete_immediately_when_no_interceptors()
        {
            // Arrange
            var interceptors = Array.Empty<IUnitOfWorkInterceptor>();
            var executor = new UnitOfWorkExecutor(interceptors);

            // Act
            var task = executor.OnAfterCommitAsync(UnitOfWork);

            // Assert
            Assert.True(task.IsCompletedSuccessfully);
            await task;
        }

        [Fact]
        public async Task Should_execute_interceptors_concurrently()
        {
            // Arrange
            var tcs1 = new TaskCompletionSource();
            var tcs2 = new TaskCompletionSource();

            Interceptor1.OnAfterCommitAsync(UnitOfWork, Arg.Any<CancellationToken>()).Returns(tcs1.Task);
            Interceptor2.OnAfterCommitAsync(UnitOfWork, Arg.Any<CancellationToken>()).Returns(tcs2.Task);

            // Act
            var task = Sut.OnAfterCommitAsync(UnitOfWork);

            // complete interceptors in reverse order to verify concurrency
            tcs2.SetResult();
            tcs1.SetResult();

            await task;

            // Assert
            await Interceptor1.Received(1).OnAfterCommitAsync(UnitOfWork, Arg.Any<CancellationToken>());
            await Interceptor2.Received(1).OnAfterCommitAsync(UnitOfWork, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_propagate_exceptions_from_interceptors()
        {
            // Arrange
            var exception = new InvalidOperationException("interceptor failed");
            Interceptor2.OnAfterCommitAsync(UnitOfWork, Arg.Any<CancellationToken>()).Throws(exception);

            // Act & Assert
            var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.OnAfterCommitAsync(UnitOfWork));

            Assert.Same(exception, actual);
        }
    }
}
