using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Giantnodes.Infrastructure.Tests;

public sealed class UnitOfWorkFactoryTests
{
    [Fact]
    public async Task Should_create_and_return_unit_of_work()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IUnitOfWorkExecutor, UnitOfWorkExecutor>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        var provider = services.BuildServiceProvider();
        var sut = new UnitOfWorkFactory(provider);

        // Act
        var result = await sut.CreateAsync();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<InMemoryUnitOfWork>(result);
    }

    [Fact]
    public async Task Should_create_multiple_independent_unit_of_works()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IUnitOfWorkExecutor, UnitOfWorkExecutor>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        var provider = services.BuildServiceProvider();

        // Act
        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();

        var factory1 = new UnitOfWorkFactory(scope1.ServiceProvider);
        var factory2 = new UnitOfWorkFactory(scope2.ServiceProvider);

        var result1 = await factory1.CreateAsync();
        var result2 = await factory2.CreateAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotSame(result1, result2);
    }

    [Fact]
    public async Task Should_handle_cancellation_token()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IUnitOfWorkExecutor, UnitOfWorkExecutor>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        var provider = services.BuildServiceProvider();
        var sut = new UnitOfWorkFactory(provider);
        var cancellation = CancellationToken.None;

        // Act
        var result = await sut.CreateAsync(cancellation);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Should_dispose_scope_when_unit_of_work_is_disposed()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IUnitOfWorkExecutor, UnitOfWorkExecutor>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        var provider = services.BuildServiceProvider();
        var sut = new UnitOfWorkFactory(provider);

        // Act
        var uow = await sut.CreateAsync();

        // verify scope is created and unit of work is functional
        Assert.NotNull(uow);

        // dispose the unit of work
        uow.Dispose();

        // Assert
        Assert.Equal(UnitOfWorkState.Disposed, uow.State);
    }
}
