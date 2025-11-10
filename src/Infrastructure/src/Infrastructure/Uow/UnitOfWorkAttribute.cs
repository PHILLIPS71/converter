using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Giantnodes.Infrastructure;

#pragma warning disable CS0649, CS8618

/// <summary>
/// Aspect that automatically wraps method execution in a Unit of Work transaction. The transaction is committed upon
/// successful completion or rolled back on exceptions.
/// </summary>
/// <remarks>
/// This attribute can only be applied to async methods. The Unit of Work is automatically created at method entry and
/// disposed at method exit, ensuring proper transaction lifecycle. If the method completes successfully without
/// manually committing, the transaction is automatically committed before disposal.
/// </remarks>
/// <example>
/// <code>
/// [UnitOfWork]
/// public async Task CreateUserAsync(CreateUserRequest request, CancellationToken cancellation)
/// {
///     // This method automatically runs within a Unit of Work transaction
///     var user = new User(request.Email);
///     await _userRepository.AddAsync(user, cancellation);
///     // Transaction is automatically committed if no exceptions occur
/// }
/// </code>
/// </example>
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
    /// Wraps the decorated async method in a unit of work transaction and automatically commits the transaction if not
    /// already committed.
    /// </summary>
    /// <returns>The result of the wrapped method.</returns>
    public override async Task<dynamic?> OverrideAsyncMethod()
    {
        var parameter = meta.Target.Parameters.LastOrDefault(p => p.Type.IsConvertibleTo(typeof(CancellationToken)));

        var cancellation = CancellationToken.None;
        if (parameter != null)
            cancellation = parameter.Value;

        using (_context = await _uow.BeginAsync(cancellation))
        {
            var result = await meta.ProceedAsync();

            if (_context.State != UnitOfWorkState.Committed)
                await _context.CommitAsync(cancellation);

            return result;
        }
    }
}
