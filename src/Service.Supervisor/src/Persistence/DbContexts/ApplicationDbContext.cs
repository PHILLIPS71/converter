using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
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

        builder.ApplyConfigurationsFromAssembly(typeof(Project.Persistence).Assembly);
    }

    public DbSet<Library> Libraries => Set<Library>();

    public DbSet<FileSystemEntry> Entries => Set<FileSystemEntry>();
    public DbSet<FileSystemDirectory> Directories => Set<FileSystemDirectory>();
    public DbSet<FileSystemFile> Files => Set<FileSystemFile>();
}