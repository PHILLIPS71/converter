using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Giantnodes.Infrastructure.Tests;

public sealed class UnitOfWorkConfiguratorTests
{
    [Fact]
    public void Should_register_interceptor_as_enumerable_service()
    {
        // Arrange
        var services = new ServiceCollection();
        var sut = new UnitOfWorkConfigurator(services);

        // Act
        sut.TryAddInterceptor<TestInterceptor>();

        // Assert
        Assert.Single(services, x =>
            x.ServiceType == typeof(IUnitOfWorkInterceptor)
            && x.ImplementationType == typeof(TestInterceptor)
            && x.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_allow_multiple_interceptor_registrations()
    {
        // Arrange
        var services = new ServiceCollection();
        var sut = new UnitOfWorkConfigurator(services);

        // Act
        sut.TryAddInterceptor<TestInterceptor>();
        sut.TryAddInterceptor<AnotherTestInterceptor>();

        // Assert
        var interceptors = services.Where(x => x.ServiceType == typeof(IUnitOfWorkInterceptor)).ToArray();
        Assert.Equal(2, interceptors.Length);
    }

    [Fact]
    public void Should_prevent_duplicate_interceptor_registrations()
    {
        // Arrange
        var services = new ServiceCollection();
        var sut = new UnitOfWorkConfigurator(services);

        // Act
        sut.TryAddInterceptor<TestInterceptor>();
        sut.TryAddInterceptor<TestInterceptor>(); // duplicate

        // Assert
        var interceptors = services
            .Where(x =>
                x.ServiceType == typeof(IUnitOfWorkInterceptor)
            && x.ImplementationType == typeof(TestInterceptor))
            .ToArray();

        Assert.Single(interceptors);
    }

    [Fact]
    public void Should_register_unit_of_work_provider_as_scoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var sut = new UnitOfWorkConfigurator(services);

        // Act
        sut.TryAddProvider<InMemoryUnitOfWork>();

        // Assert
        Assert.Single(services, x =>
            x.ServiceType == typeof(IUnitOfWork)
            && x.ImplementationType == typeof(InMemoryUnitOfWork)
            && x.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void Should_prevent_duplicate_provider_registrations()
    {
        // Arrange
        var services = new ServiceCollection();
        var sut = new UnitOfWorkConfigurator(services);

        // Act
        sut.TryAddProvider<InMemoryUnitOfWork>();
        sut.TryAddProvider<TestUnitOfWork>(); // should be ignored

        // Assert
        var descriptors = services.Where(x => x.ServiceType == typeof(IUnitOfWork)).ToArray();
        Assert.Single(descriptors);

        var descriptor = descriptors.First();
        Assert.Equal(typeof(InMemoryUnitOfWork), descriptor.ImplementationType);
    }

    [Fact]
    public void Should_return_configurator_for_method_chaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var sut = new UnitOfWorkConfigurator(services);

        // Act
        var result = sut.TryAddInterceptor<TestInterceptor>();

        // Assert
        Assert.Same(sut, result);
    }

    [Fact]
    public void Should_support_method_chaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var sut = new UnitOfWorkConfigurator(services);

        // Act & Assert
        var result = sut
            .TryAddProvider<InMemoryUnitOfWork>()
            .TryAddInterceptor<TestInterceptor>()
            .TryAddInterceptor<AnotherTestInterceptor>();

        Assert.Same(sut, result);

        var providers = services.Count(x => x.ServiceType == typeof(IUnitOfWork));
        var interceptors = services.Count(x => x.ServiceType == typeof(IUnitOfWorkInterceptor));

        Assert.Equal(1, providers);
        Assert.Equal(2, interceptors);
    }

    // test implementations
    private sealed class TestInterceptor : IUnitOfWorkInterceptor
    {
        public Task OnAfterBeginAsync(UnitOfWork uow, CancellationToken cancellation = default) => Task.CompletedTask;
        public Task OnAfterCommitAsync(UnitOfWork uow, CancellationToken cancellation = default) => Task.CompletedTask;
    }

    private sealed class AnotherTestInterceptor : IUnitOfWorkInterceptor
    {
        public Task OnAfterBeginAsync(UnitOfWork uow, CancellationToken cancellation = default) => Task.CompletedTask;
        public Task OnAfterCommitAsync(UnitOfWork uow, CancellationToken cancellation = default) => Task.CompletedTask;
    }

    private sealed class TestUnitOfWork : UnitOfWork
    {
        public TestUnitOfWork(IUnitOfWorkExecutor executor) : base(executor)
        {
        }

        protected override Task OnBeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default) =>
            Task.CompletedTask;

        protected override Task OnCommitAsync(CancellationToken cancellation = default) => Task.CompletedTask;
        protected override Task OnRollbackAsync(CancellationToken cancellation = default) => Task.CompletedTask;
    }
}
