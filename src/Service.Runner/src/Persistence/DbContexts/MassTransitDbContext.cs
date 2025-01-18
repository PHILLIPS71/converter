using Giantnodes.Infrastructure.EntityFrameworkCore;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Runner.Persistence.DbContexts;

public class MassTransitDbContext : GiantnodesDbContext<MassTransitDbContext>
{
    internal const string Schema = "masstransit";

    public MassTransitDbContext(DbContextOptions<MassTransitDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var saga in Sagas)
            saga.Configure(modelBuilder);

        modelBuilder.AddTransactionalOutboxEntities();

        modelBuilder.HasDefaultSchema(Schema);
    }

    private static IEnumerable<ISagaClassMap> Sagas
    {
        get
        {
            yield return new JobTypeSagaMap(true);
            yield return new JobSagaMap(true);
            yield return new JobAttemptSagaMap(true);
        }
    }
}