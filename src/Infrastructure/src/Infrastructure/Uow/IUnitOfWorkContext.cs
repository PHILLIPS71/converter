namespace Giantnodes.Infrastructure;

public interface IUnitOfWorkContext : IDisposable
{
    Guid CorrelationId { get; }

    Guid? UserId { get; }

    Task CommitAsync(CancellationToken cancellation = default);
}