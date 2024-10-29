﻿using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

public sealed class Library : AggregateRoot<Guid>, ITimestampableEntity
{
    private Library()
    {
    }

    private Library(FileSystemDirectory directory, LibraryName name)
    {
        Id = NewId.NextSequentialGuid();
        Name = name;
        Slug = LibrarySlug.Create(name);
        Directory = directory;
    }

    public static ErrorOr<Library> Create(FileSystemDirectory directory, LibraryName name)
    {
        var library = new Library(directory, name);

        var @event = new LibraryCreatedEvent
        {
            LibraryId = library.Id,
            DirectoryId = library.Directory.Id,
            Name = library.Name.Value,
            Slug = library.Slug.Value,
        };

        library.DomainEvents.Add(@event);
        return library;
    }

    public LibraryName Name { get; private set; }

    public LibrarySlug Slug { get; private set; }

    public FileSystemDirectory Directory { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}