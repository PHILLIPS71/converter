using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

public sealed class Library : AggregateRoot<Id>, ITimestampableEntity
{
    private Library()
    {
    }

    private Library(FileSystemDirectory directory, Name name, Slug slug)
    {
        Id = Id.NewId();
        Name = name;
        Slug = slug;
        Directory = directory;
    }

    public static ErrorOr<Library> Create(FileSystemDirectory directory, Name name, Slug slug)
    {
        var library = new Library(directory, name, slug);

        var @event = new LibraryCreatedEvent
        {
            LibraryId = library.Id,
            DirectoryId = library.Directory.Id,
            Name = library.Name.Value,
            Slug = library.Slug.Value
        };

        library.DomainEvents.Add(@event);
        return library;
    }

    public void SetMonitoring(bool monitoring)
    {
        IsMonitoring = monitoring;

        DomainEvents.Add(new LibraryMonitoringChangedEvent
        {
            LibraryId = Id,
            IsMonitoring = IsMonitoring
        });
    }

    public Name Name { get; private set; }

    public Slug Slug { get; private set; }

    public Id DirectoryId { get; private set; }
    public FileSystemDirectory Directory { get; private set; }

    public bool IsMonitoring { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}
