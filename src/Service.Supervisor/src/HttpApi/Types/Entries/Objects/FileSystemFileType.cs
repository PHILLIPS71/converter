using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.HttpApi.Types.Entries.Interfaces;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Entries.Objects;

[ObjectType<FileSystemFile>]
public static partial class FileSystemFileType
{
    static partial void Configure(IObjectTypeDescriptor<FileSystemFile> descriptor)
    {
        descriptor.Implements<FileSystemEntryType>();

        descriptor
            .Field(f => f.Id);

        descriptor
            .Field(f => f.PathInfo);

        descriptor
            .Field(f => f.Size);

        descriptor
            .Field(f => f.CreatedAt);

        descriptor
            .Field(f => f.UpdatedAt);
    }

    [NodeResolver]
    internal static Task<FileSystemFile?> GetFileSystemFileByIdAsync(
        Guid id,
        ISelection selection,
        IFileSystemFileByIdDataLoader dataloader,
        CancellationToken cancellation)
    {
        return dataloader.Select(selection).LoadAsync(id, cancellation);
    }

    [DataLoader]
    internal static Task<Dictionary<Guid, FileSystemFile>> GetFileSystemFileByIdAsync(
        IReadOnlyList<Guid> keys,
        ISelectorBuilder selector,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .Files
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .Select(x => x.Id, selector)
            .ToDictionaryAsync(x => x.Id, cancellation);
    }
}