using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Giantnodes.Infrastructure;

#pragma warning disable CS0649, CS8618

public sealed class UnitOfWorkAttribute : OverrideMethodAspect
{
    [IntroduceDependency]
    private readonly IUnitOfWorkService _uow;

    [Introduce(WhenExists = OverrideStrategy.Override)]
    private IUnitOfWorkContext _context;

    public override dynamic OverrideMethod() => throw new NotImplementedException();

    public override async Task<dynamic?> OverrideAsyncMethod()
    {
        var parameter = meta.Target.Parameters.LastOrDefault(p => p.Type.Is(typeof(CancellationToken)));

        var cancellation = CancellationToken.None;
        if (parameter != null)
            cancellation = parameter.Value;

        _context = await _uow.BeginAsync(cancellation);
        var result = await meta.ProceedAsync();
        await _context.CommitAsync(cancellation);

        return result;
    }
}