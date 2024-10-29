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

        var library = await _service.CreateAsync(name.Value, context.Message.Path, context.CancellationToken);
        if (library.IsError)
        {
            await context.RejectAsync(FaultKind.Constraint, library.ToFault());
            return;
        }

        await context.RespondAsync(new LibraryCreate.Result { Id = library.Value.Id });
    }
}