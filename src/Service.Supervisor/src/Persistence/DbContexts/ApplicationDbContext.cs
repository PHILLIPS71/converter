using Giantnodes.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Persistence.DbContexts;

public sealed class ApplicationDbContext : GiantnodesDbContext<ApplicationDbContext>
{
    internal const string Schema = "public";

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(Schema);
        builder.HasPostgresExtension("citext");
    }
}