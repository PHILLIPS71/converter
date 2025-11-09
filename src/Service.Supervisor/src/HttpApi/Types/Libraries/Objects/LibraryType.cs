using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Libraries.Objects;

[ObjectType<Library>]
public static partial class LibraryType
{
    static partial void Configure(IObjectTypeDescriptor<Library> descriptor)
    {
        descriptor
            .Field(f => f.Id);

        descriptor
            .Field(f => f.Name);

        descriptor
            .Field(f => f.Slug);

        descriptor
            .Field(f => f.Directory);

        descriptor
            .Field(f => f.CreatedAt);

        descriptor
            .Field(f => f.UpdatedAt);
    }

    [NodeResolver]
    public static Task<Library?> GetLibraryByIdAsync(
        Id id,
        QueryContext<Library> query,
        ILibraryByIdDataLoader dataloader,
        CancellationToken cancellation)
    {
        return dataloader.With(query).LoadAsync(id, cancellation);
    }

    [DataLoader]
    internal static Task<Dictionary<Id, Library>> GetLibraryByIdAsync(
        IReadOnlyList<Id> keys,
        QueryContext<Library> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .Libraries
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query, x => x.AddAscending(y => y.Name))
            .ToDictionaryAsync(x => x.Id, cancellation);
    }
}
