using MassTransit;

namespace Giantnodes.Infrastructure.MassTransit;

/// <summary>
/// Implements a Unit of Work interceptor that publishes events after a successful commit.
/// </summary>
public sealed class PublishUnitOfWorkInterceptor : IUnitOfWorkInterceptor
{
    private readonly IBus _bus;

    public PublishUnitOfWorkInterceptor(IBus bus)
    {
        _bus = bus;
    }

    /// <inheritdoc />
    public Task OnAfterBeginAsync(UnitOfWork uow, CancellationToken cancellation = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task OnAfterCommitAsync(UnitOfWork uow, CancellationToken cancellation = default)
    {
        if (uow.Events.Count == 0)
            return Task.CompletedTask;

        return _bus.PublishBatch(uow.Events, cancellation);
    }
}