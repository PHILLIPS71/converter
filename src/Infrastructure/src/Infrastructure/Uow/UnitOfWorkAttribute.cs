using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Giantnodes.Infrastructure;

#pragma warning disable CS0649, CS8618

public sealed class UnitOfWorkAttribute : OverrideMethodAspect
{
    /// <summary>
    /// The unit of work service that will be injected by the aspect framework.
    /// </summary>
    [IntroduceDependency]
    private readonly IUnitOfWorkService _uow;

    /// <summary>
    /// The current unit of work context.
    /// </summary>
    [Introduce(WhenExists = OverrideStrategy.Override)]
    private IUnitOfWorkContext _context;

    /// <summary>
    /// Override for synchronous methods. Not supported in this implementation.
    /// </summary>
    /// <exception cref="NotImplementedException">Always thrown as only async methods are supported.</exception>
    public override dynamic OverrideMethod() => throw new NotImplementedException();

    /// <summary>
    /// Wraps the decorated async method in a unit of work transaction and automatically commits the transaction
    /// if not already committed.
    /// </summary>
    /// <returns>The result of the wrapped method.</returns>
    public override async Task<dynamic?> OverrideAsyncMethod()
    {
        var parameter = meta.Target.Parameters.LastOrDefault(p => p.Type.IsConvertibleTo(typeof(CancellationToken)));

        var cancellation = CancellationToken.None;
        if (parameter != null)
            cancellation = parameter.Value;

        await using (_context = await _uow.BeginAsync(cancellation))
        {
            var result = await meta.ProceedAsync();

            if (!_context.IsCommitted)
                await _context.CommitAsync(cancellation);

            return result;
        }
    }
}
