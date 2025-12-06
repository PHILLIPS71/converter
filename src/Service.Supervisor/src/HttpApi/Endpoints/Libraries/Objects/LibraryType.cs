using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Libraries;

[ObjectType<Library>]
internal static partial class LibraryType
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
    public static async Task<Library?> GetLibraryByIdAsync(
        Id id,
        QueryContext<Library> query,
        ILibraryByIdDataLoader dataloader,
        CancellationToken cancellation)
        => await dataloader
            .With(query)
            .LoadAsync(id, cancellation);

    [DataLoader]
    internal static async Task<Dictionary<Id, Library>> GetLibraryByIdAsync(
        IReadOnlyList<Id> keys,
        QueryContext<Library> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
        => await database
            .Libraries
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query)
            .ToDictionaryAsync(x => x.Id, cancellation);
}
