using Giantnodes.Infrastructure.EntityFrameworkCore;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Persistence.DbContexts;

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

        modelBuilder.AddTransactionalOutboxEntities();

        modelBuilder.HasDefaultSchema(Schema);
    }
}