using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure;

/// <summary>
/// Internal implementation of Unit of Work configuration that provides fluent API for service registration.
/// </summary>
internal sealed class UnitOfWorkConfigurator : ServiceCollectionConfigurator, IUnitOfWorkConfigurator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkConfigurator"/> class.
    /// </summary>
    /// <param name="collection">The service collection to configure.</param>
    public UnitOfWorkConfigurator(IServiceCollection collection)
        : base(collection)
    {
    }

    /// <inheritdoc />
    public IUnitOfWorkConfigurator TryAddInterceptor<TInterceptor>()
        where TInterceptor : class, IUnitOfWorkInterceptor
    {
        // use enumerable registration to allow multiple interceptors
        Services.TryAddEnumerable(ServiceDescriptor.Scoped<IUnitOfWorkInterceptor, TInterceptor>());

        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkConfigurator TryAddProvider<TUnitOfWork>()
        where TUnitOfWork : IUnitOfWork
    {
        // register as scoped to maintain state within request/operation scope
        Services.TryAddScoped(typeof(IUnitOfWork), typeof(TUnitOfWork));

        return this;
    }
}
