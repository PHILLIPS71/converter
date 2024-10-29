using System.IO.Abstractions;
using EntityFramework.Exceptions.Common;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Libraries.Commands;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using MassTransit;
using Npgsql;

namespace Giantnodes.Service.Supervisor.Components.Libraries.Commands;

public sealed partial class LibraryCreateConsumer : IConsumer<LibraryCreate.Command>
{
    private readonly IUnitOfWorkService _uow;
    private readonly IFileSystem _fs;
    private readonly ILibraryRepository _libraries;
    private readonly IDirectoryRepository _directories;

    public LibraryCreateConsumer(
        IUnitOfWorkService uow,
        IFileSystem fs,
        ILibraryRepository libraries,
        IDirectoryRepository directories)
    {
        _uow = uow;
        _fs = fs;
        _libraries = libraries;
        _directories = directories;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<LibraryCreate.Command> context)
    {
        FileSystemDirectory? entry;
        Library? library;

        await using (var uow = await _uow.BeginAsync(context.CancellationToken))
        {
            var directory = _fs.DirectoryInfo.New(context.Message.Path);
            if (!directory.Exists)
            {
                await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(context.Message.Path));
                return;
            }

            entry = await _directories.SingleOrDefaultAsync(x => x.PathInfo.FullName == context.Message.Path);
            if (entry == null)
            {
                entry = new FileSystemDirectory(directory);
                _directories.Create(entry);

                await uow.CommitAsync(context.CancellationToken);
            }
        }

        await using (var uow = await _uow.BeginAsync(context.CancellationToken))
        {
            try
            {
                library = new Library(entry, context.Message.Name);

                _libraries.Create(library);
                await uow.CommitAsync(context.CancellationToken);
            }
            catch (UniqueConstraintException ex) when (ex.InnerException is PostgresException pg)
            {
                var fault = pg.ConstraintName switch
                {
                    "ix_libraries_slug" => FaultProperty.Create(context.Message.Path),
                    "ix_libraries_name" => FaultProperty.Create(context.Message.Path),
                    _ => null
                };
                
                await _context.RollbackAsync(context.CancellationToken);
                await context.RejectAsync(FaultKind.Constraint, fault);
                return;
            }
            catch (Exception)
            {
                await _context.RollbackAsync(context.CancellationToken);
                await context.RejectAsync(FaultKind.Unexpected);
                return;
            }
        }

        await context.RespondAsync(new LibraryCreate.Result { Id = library.Id });
    }
}