using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Giantnodes.Infrastructure.EntityFrameworkCore;

#pragma warning disable S3011
/// <summary>
/// A custom implementation of <see cref="DbContext"/> that provides additional functionality for auditing,
/// soft deletes, and concurrency handling.
/// </summary>
/// <typeparam name="TDbContext">The type of the derived <see cref="DbContext"/> class.</typeparam>
public class PlatformDbContext<TDbContext> : DbContext
    where TDbContext : DbContext
{
    private static readonly MethodInfo? ConfigureAuditPropertiesMethodInfo
        = typeof(PlatformDbContext<TDbContext>)
            .GetMethod(nameof(ConfigureAuditProperties), BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo? ConfigureConcurrencyTokenMethodInfo
        = typeof(PlatformDbContext<TDbContext>)
            .GetMethod(nameof(ConfigureConcurrencyToken), BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformDbContext{TDbContext}"/> class.
    /// </summary>
    /// <param name="options">The options to be used by this <see cref="DbContext"/>.</param>
    protected PlatformDbContext(DbContextOptions<TDbContext> options)
        : base(options)
    {
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        base.ConfigureConventions(builder);

        builder
            .Properties<Id>()
            .HaveConversion<IdValueConverter>()
            .HaveMaxLength(26)
            .AreFixedLength();

        builder
            .Properties(typeof(Id<>))
            .HaveConversion(typeof(IdValueConverter<>))
            .HaveMaxLength(26)
            .AreFixedLength();

        builder.Conventions.Add(_ => new IdConvention());
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var type in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureAuditPropertiesMethodInfo?
                .MakeGenericMethod(type.ClrType)
                .Invoke(this, [modelBuilder, type]);

            ConfigureConcurrencyTokenMethodInfo?
                .MakeGenericMethod(type.ClrType)
                .Invoke(this, [modelBuilder, type]);
        }
    }

    /// <inheritdoc />
    public override int SaveChanges()
    {
        NullifyEmptyStrings();

        SetTimestampEntityProperties();
        SetSoftDeleteEntityProperties();

        return base.SaveChanges();
    }

    /// <inheritdoc />
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NullifyEmptyStrings();

        SetTimestampEntityProperties();
        SetSoftDeleteEntityProperties();

        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Configures audit properties for the specified <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    /// <param name="mutable">The mutable entity type for the <typeparamref name="TEntity"/>.</param>
    /// <typeparam name="TEntity">The entity type to configure audit properties for.</typeparam>
    protected void ConfigureAuditProperties<TEntity>(ModelBuilder builder, IMutableEntityType mutable)
        where TEntity : class
    {
        if (mutable.IsOwned())
            return;

        if (!typeof(IEntity).IsAssignableFrom(typeof(TEntity)))
            return;

        ConfigureGlobalFilters<TEntity>(builder);
    }

    /// <summary>
    /// Configures the <see cref="IHasConcurrencyToken.ConcurrencyToken"/> property for entities implementing
    /// <see cref="IHasConcurrencyToken"/> to be a row version, enabling optimistic concurrency control.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    /// <param name="mutable">The mutable entity type for the <typeparamref name="TEntity"/>.</param>
    /// <typeparam name="TEntity">The entity type to configure the concurrency token property for.</typeparam>
    protected void ConfigureConcurrencyToken<TEntity>(ModelBuilder builder, IMutableEntityType mutable)
        where TEntity : class
    {
        if (mutable.IsOwned())
            return;

        if (!typeof(IHasConcurrencyToken).IsAssignableFrom(typeof(TEntity)))
            return;

        builder
            .Entity<TEntity>()
            .Property(nameof(IHasConcurrencyToken.ConcurrencyToken))
            .IsRowVersion();
    }

    /// <summary>
    /// Configures global filters for the specified <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder being used to construct the model for this context.</param>
    /// <typeparam name="TEntity">The entity type to configure global filters for.</typeparam>
    private static void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? expression = null;

        if (typeof(ISoftDeletableEntity).IsAssignableFrom(typeof(TEntity)))
            expression = e => !EF.Property<bool>(e, nameof(ISoftDeletableEntity.IsDeleted));

        if (expression != null)
            modelBuilder.Entity<TEntity>().HasQueryFilter(expression);
    }

    /// <summary>
    /// Nullifies empty strings for string properties on tracked entities.
    /// </summary>
    private void NullifyEmptyStrings()
    {
        foreach (var entity in ChangeTracker.Entries())
        {
            var properties = entity
                .Entity
                .GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                property.SetValue(entity.Entity, (property.GetValue(entity.Entity) as string)?.Trim());

                if (string.IsNullOrWhiteSpace(property.GetValue(entity.Entity) as string))
                    property.SetValue(entity.Entity, null, null);
            }
        }
    }

    /// <summary>
    /// Sets the timestamp entity properties for tracked entities implementing <see cref="ITimestampableEntity"/>.
    /// </summary>
    private void SetTimestampEntityProperties()
    {
        foreach (var entry in ChangeTracker.Entries<ITimestampableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ObjectHelper.SetProperty(entry.Entity, x => x.CreatedAt, _ => DateTime.UtcNow);
                    break;

                case EntityState.Modified:
                    ObjectHelper.SetProperty(entry.Entity, x => x.UpdatedAt, _ => DateTime.UtcNow);
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the soft delete entity properties for tracked entities implementing <see cref="ISoftDeletableEntity"/>.
    /// </summary>
    private void SetSoftDeleteEntityProperties()
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDeletableEntity>())
        {
            if (entry.State != EntityState.Deleted)
                continue;

            entry.Reload();
            ObjectHelper.SetProperty(entry.Entity, x => x.IsDeleted, _ => true);
            ObjectHelper.SetProperty(entry.Entity, x => x.DeletedAt, _ => DateTime.UtcNow);
        }
    }
}
