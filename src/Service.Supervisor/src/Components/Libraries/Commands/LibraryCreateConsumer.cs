using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Libraries.Commands;

public sealed partial class LibraryCreateConsumer : IConsumer<LibraryCreate.Command>
{
    private readonly ILibraryService _service;

    public LibraryCreateConsumer(ILibraryService service)
    {
        _service = service;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<LibraryCreate.Command> context)
    {
        var name = LibraryName.Create(context.Message.Name);
        if (name.IsError)
        {
            await context.RejectAsync(FaultKind.Validation, name.ToFault());
            return;
        }

        var slug = LibrarySlug.Create(name.Value);
        if (slug.IsError)
        {
            await context.RejectAsync(FaultKind.Validation, slug.ToFault());
            return;
        }

        var library = await _service.CreateAsync(name.Value, slug.Value, context.Message.Path, context.CancellationToken);
        if (library.IsError)
        {
            await context.RejectAsync(library.ToFaultKind(), library.ToFault());
            return;
        }

        if (context.Message.IsMonitoring)
            library.Value.SetMonitoring(true);

        await context.RespondAsync(new LibraryCreate.Result { LibraryId = library.Value.Id });
    }
}
