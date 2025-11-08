using System.Transactions;
using NSubstitute;
using Xunit;

namespace Giantnodes.Infrastructure.Tests;

public abstract class UnitOfWorkServiceFixture
{
    internal readonly UnitOfWorkService Sut;

    protected readonly IUnitOfWorkFactory Factory;
    protected readonly IUnitOfWorkAccessor Accessor;
    protected readonly IUnitOfWork UnitOfWork;

    protected UnitOfWorkServiceFixture()
    {
        Factory = Substitute.For<IUnitOfWorkFactory>();
        Accessor = Substitute.For<IUnitOfWorkAccessor>();
        UnitOfWork = Substitute.For<IUnitOfWork>();

        Sut = new UnitOfWorkService(Factory, Accessor);
    }
}

public sealed class UnitOfWorkServiceTests : UnitOfWorkServiceFixture
{
    public sealed class BeginAsyncWithDefaults : UnitOfWorkServiceFixture
    {
        [Fact]
        public async Task Should_begin_with_default_required_scope()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWork>();

            Factory
                .CreateAsync(Arg.Any<CancellationToken>())
                .Returns(UnitOfWork);

            UnitOfWork
                .BeginAsync(Arg.Any<UnitOfWorkOptions>(), Arg.Any<CancellationToken>())
                .Returns(context);

            // Act
            await Sut.BeginAsync();

            // Assert
            await UnitOfWork
                .Received(1)
                .BeginAsync(
                    Arg.Is<UnitOfWorkOptions>(x => x.Scope == TransactionScopeOption.Required),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_set_context_in_accessor()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWork>();

            Factory
                .CreateAsync(Arg.Any<CancellationToken>())
                .Returns(UnitOfWork);

            UnitOfWork
                .BeginAsync(Arg.Any<UnitOfWorkOptions>(), Arg.Any<CancellationToken>())
                .Returns(context);

            // Act
            await Sut.BeginAsync();

            // Assert
            Accessor.Received(1).SetCurrent(context);
        }

        [Fact]
        public async Task Should_return_unit_of_work_context()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWork>();

            Factory
                .CreateAsync(Arg.Any<CancellationToken>())
                .Returns(UnitOfWork);

            UnitOfWork
                .BeginAsync(Arg.Any<UnitOfWorkOptions>(), Arg.Any<CancellationToken>())
                .Returns(context);

            // Act
            var result = await Sut.BeginAsync();

            // Assert
            Assert.Same(context, result);
        }

        [Fact]
        public async Task Should_use_cancellation_token()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWork>();
            var cancellation = CancellationToken.None;

            Factory
                .CreateAsync(cancellation)
                .Returns(UnitOfWork);

            UnitOfWork
                .BeginAsync(Arg.Any<UnitOfWorkOptions>(), cancellation)
                .Returns(context);

            // Act
            await Sut.BeginAsync(cancellation);

            // Assert
            await Factory.Received(1).CreateAsync(cancellation);
            await UnitOfWork.Received(1).BeginAsync(Arg.Any<UnitOfWorkOptions>(), cancellation);
        }
    }

    public sealed class BeginAsyncWithOptions : UnitOfWorkServiceFixture
    {
        [Fact]
        public async Task Should_throw_argument_null_exception_when_options_is_null()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => Sut.BeginAsync(null!));
        }

        [Fact]
        public async Task Should_begin_with_provided_options()
        {
            // Arrange
            var options = new UnitOfWorkOptions { Scope = TransactionScopeOption.RequiresNew };
            var context = Substitute.For<IUnitOfWork>();

            Factory
                .CreateAsync(Arg.Any<CancellationToken>())
                .Returns(UnitOfWork);

            UnitOfWork
                .BeginAsync(options, Arg.Any<CancellationToken>())
                .Returns(context);

            // Act
            var result = await Sut.BeginAsync(options);

            // Assert
            await UnitOfWork.Received(1).BeginAsync(options, Arg.Any<CancellationToken>());
            Assert.Same(context, result);
        }

        [Fact]
        public async Task Should_set_context_in_accessor()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            var context = Substitute.For<IUnitOfWork>();

            Factory
                .CreateAsync(Arg.Any<CancellationToken>())
                .Returns(UnitOfWork);

            UnitOfWork
                .BeginAsync(options, Arg.Any<CancellationToken>())
                .Returns(context);

            // Act
            await Sut.BeginAsync(options);

            // Assert
            Accessor.Received(1).SetCurrent(context);
        }

        [Fact]
        public async Task Should_use_cancellation_token()
        {
            // Arrange
            var options = new UnitOfWorkOptions();
            var context = Substitute.For<IUnitOfWork>();
            var cancellation = CancellationToken.None;

            Factory.CreateAsync(cancellation).Returns(UnitOfWork);
            UnitOfWork.BeginAsync(options, cancellation).Returns(context);

            // Act
            await Sut.BeginAsync(options, cancellation);

            // Assert
            await Factory.Received(1).CreateAsync(cancellation);
            await UnitOfWork.Received(1).BeginAsync(options, cancellation);
        }
    }

    public sealed class EventHandling : UnitOfWorkServiceFixture
    {
        [Fact]
        public async Task Should_clear_accessor_on_completed_event()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWork>();

            Factory
                .CreateAsync(Arg.Any<CancellationToken>())
                .Returns(UnitOfWork);

            UnitOfWork
                .BeginAsync(Arg.Any<UnitOfWorkOptions>(), Arg.Any<CancellationToken>())
                .Returns(context);

            await Sut.BeginAsync();

            // Act
            UnitOfWork.Completed += Raise.Event<EventHandler>(UnitOfWork, EventArgs.Empty);

            // Assert
            Accessor.Received(1).SetCurrent(null);
        }

        [Fact]
        public async Task Should_clear_accessor_on_failed_event()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWork>();

            Factory
                .CreateAsync(Arg.Any<CancellationToken>())
                .Returns(UnitOfWork);

            UnitOfWork
                .BeginAsync(Arg.Any<UnitOfWorkOptions>(), Arg.Any<CancellationToken>())
                .Returns(context);

            await Sut.BeginAsync();

            // Act
            UnitOfWork.Failed += Raise.Event<EventHandler>(UnitOfWork, EventArgs.Empty);

            // Assert
            Accessor.Received(1).SetCurrent(null);
        }

        [Fact]
        public async Task Should_clear_accessor_on_disposed_event()
        {
            // Arrange
            var context = Substitute.For<IUnitOfWork>();

            Factory
                .CreateAsync(Arg.Any<CancellationToken>())
                .Returns(UnitOfWork);

            UnitOfWork
                .BeginAsync(Arg.Any<UnitOfWorkOptions>(), Arg.Any<CancellationToken>())
                .Returns(context);

            await Sut.BeginAsync();

            // Act
            UnitOfWork.Disposed += Raise.Event<EventHandler>(UnitOfWork, EventArgs.Empty);

            // Assert
            Accessor.Received(1).SetCurrent(null);
        }
    }
}
