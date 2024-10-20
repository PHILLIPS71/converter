using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using MassTransit;
using Slugify;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

public sealed class Library : AggregateRoot<Guid>, ITimestampableEntity
{
    private Library()
    {
    }

    public Library(FileSystemDirectory directory, string name)
    {
        Id = NewId.NextSequentialGuid();
        Name = name;
        Slug = new SlugHelper().GenerateSlug(name);
        Directory = directory;
    }

    public string Name { get; private set; }

    public string Slug { get; private set; }

    public FileSystemDirectory Directory { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}