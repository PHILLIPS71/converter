using System.Transactions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Giantnodes.Infrastructure.Tests;

public abstract class UnitOfWorkFixture
{
    internal readonly InMemoryUnitOfWork Sut;

    protected readonly IUnitOfWorkExecutor Executor;

    protected UnitOfWorkFixture()
    {
        Executor = Substitute.For<IUnitOfWorkExecutor>();
        Sut = new InMemoryUnitOfWork(Executor);
    }
}

public sealed class UnitOfWorkTests : UnitOfWorkFixture
{
    public sealed class Constructor : UnitOfWorkFixture
    {
        [Fact]
        public void Should_initialize_with_correlation_id_and_created_state()
        {
            // Assert
            Assert.NotEqual(Guid.Empty, Sut.CorrelationId);
            Assert.Equal(UnitOfWorkState.Created, Sut.State);
            Assert.Null(Sut.UserId);
            Assert.Null(Sut.TenantId);
            Assert.Null(Sut.Options);
            Assert.Empty(Sut.Events);
        }
    }

    public sealed class SetUserId : UnitOfWorkFixture
    {
        [Fact]
        public void Should_set_user_id()
        {
            // Arrange
            var id = Id.NewId();

            // Act
            Sut.SetUserId(id);

            // Assert
            Assert.Equal(id, Sut.UserId);
        }
    }

    public sealed class SetTenantId : UnitOfWorkFixture
    {
        [Fact]
        public void Should_set_tenant_id()
        {
            // Arrange
            var id = Id.NewId();

            // Act
            Sut.SetTenantId(id);

            // Assert
            Assert.Equal(id, Sut.TenantId);
        }
    }

    public sealed class BeginAsync : UnitOfWorkFixture
    {
        [Fact]
        public async Task Should_begin_successfully_with_valid_options()
        {
            // Arrange
            var options = new UnitOfWorkOptions { Scope = TransactionScopeOption.Required };

            // Act
            var result = await Sut.BeginAsync(options);

            // Assert
            Assert.Same(Sut, result);
            Assert.Equal(UnitOfWorkState.Started, Sut.State);
            Assert.Same(options, Sut.Options);
            await Executor.Received(1).OnAfterBeginAsync(Sut, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_throw_argument_null_exception_when_options_is_null()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => Sut.BeginAsync(null!));
        }

        [Fact]
        public async Task Should_throw_argument_out_of_range_exception_when_timeout_is_zero()
        {
            // Arrange
            var options = new UnitOfWorkOptions { Timeout = TimeSpan.Zero };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Sut.BeginAsync(options));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(-1000)]
        public async Task Should_throw_argument_out_of_range_exception_when_timeout_is_negative(int milliseconds)
        {
            // Arrange
            var options = new UnitOfWorkOptions { Timeout = TimeSpan.FromMilliseconds(milliseconds) };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Sut.BeginAsync(options));
        }

        [Fact]
        public async Task Should_throw_invalid_operation_exception_when_already_started()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            await Sut.BeginAsync(options);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.BeginAsync(options));
        }

        [Theory]
        [InlineData(TransactionScopeOption.Required)]
        [InlineData(TransactionScopeOption.RequiresNew)]
        [InlineData(TransactionScopeOption.Suppress)]
        public async Task Should_handle_different_transaction_scope_options(TransactionScopeOption scope)
        {
            // Arrange
            var options = new UnitOfWorkOptions { Scope = scope };

            // Act
            await Sut.BeginAsync(options);

            // Assert
            Assert.Equal(UnitOfWorkState.Started, Sut.State);
        }

        [Fact]
        public async Task Should_use_cancellation_token_in_executor()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            var cancellationToken = CancellationToken.None;

            // Act
            await Sut.BeginAsync(options, cancellationToken);

            // Assert
            await Executor.Received(1).OnAfterBeginAsync(Sut, cancellationToken);
        }
    }

    public sealed class CommitAsync : UnitOfWorkFixture
    {
        [Fact]
        public async Task Should_commit_successfully_when_started()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            await Sut.BeginAsync(options);

            var called = false;
            Sut.Completed += (_, _) => called = true;

            // Act
            await Sut.CommitAsync();

            // Assert
            Assert.Equal(UnitOfWorkState.Committed, Sut.State);
            Assert.True(called);
            await Executor.Received(1).OnAfterCommitAsync(Sut, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_throw_invalid_operation_exception_when_not_started()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.CommitAsync());
        }

        [Fact]
        public async Task Should_throw_invalid_operation_exception_when_already_committed()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            await Sut.BeginAsync(options);
            await Sut.CommitAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.CommitAsync());
        }

        [Fact]
        public async Task Should_use_cancellation_token_in_executor()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            await Sut.BeginAsync(options);
            var cancellation = CancellationToken.None;

            // Act
            await Sut.CommitAsync(cancellation);

            // Assert
            await Executor.Received(1).OnAfterCommitAsync(Sut, cancellation);
        }
    }

    public sealed class RollbackAsync : UnitOfWorkFixture
    {
        [Fact]
        public async Task Should_rollback_successfully_when_started()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            await Sut.BeginAsync(options);

            // Act
            await Sut.RollbackAsync();

            // Assert
            Assert.Equal(UnitOfWorkState.RolledBack, Sut.State);
        }

        [Fact]
        public async Task Should_throw_invalid_operation_exception_when_not_started()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.RollbackAsync());
        }

        [Fact]
        public async Task Should_throw_invalid_operation_exception_when_already_committed()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            await Sut.BeginAsync(options);
            await Sut.CommitAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.RollbackAsync());
        }

        [Fact]
        public async Task Should_clear_domain_events_on_rollback()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            await Sut.BeginAsync(options);

            // Act
            await Sut.RollbackAsync();

            // Assert
            Assert.Empty(Sut.Events);
        }
    }

    public sealed class Dispose : UnitOfWorkFixture
    {
        [Fact]
        public void Should_dispose_successfully_and_fire_disposed_event()
        {
            // Arrange
            var called = false;
            Sut.Disposed += (_, _) => called = true;

            // Act
            Sut.Dispose();

            // Assert
            Assert.Equal(UnitOfWorkState.Disposed, Sut.State);
            Assert.True(called);
        }

        [Fact]
        public void Should_not_dispose_multiple_times()
        {
            // Arrange
            var count = 0;
            Sut.Disposed += (_, _) => count++;

            // Act
            Sut.Dispose();
            Sut.Dispose();

            // Assert
            Assert.Equal(1, count);
            Assert.Equal(UnitOfWorkState.Disposed, Sut.State);
        }

        [Fact]
        public async Task Should_fire_failed_event_when_exception_occurred_during_lifecycle()
        {
            // Arrange
            UnitOfWorkFailedEventArgs? failure = null;
            Sut.Failed += (_, args) => failure = args as UnitOfWorkFailedEventArgs;

            // simulate an exception by making the executor throw
            var exception = new InvalidOperationException("test exception");

            Executor
                .OnAfterBeginAsync(Arg.Any<UnitOfWork>(), Arg.Any<CancellationToken>())
                .Throws(exception);

            var options = new UnitOfWorkOptions();

            // Act
            try
            {
                await Sut.BeginAsync(options);
            }
            catch
            {
                // Expected
            }

            Sut.Dispose();

            // Assert
            Assert.NotNull(failure);
            Assert.Same(exception, failure.Exception);
        }
    }
}
