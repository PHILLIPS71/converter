using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.EntityFrameworkCore;

public sealed class MigratorHostedService<TDbContext> : IHostedService
    where TDbContext : DbContext
{
    private readonly IServiceScopeFactory _factory;
    private readonly ILogger<MigratorHostedService<TDbContext>> _logger;

    public MigratorHostedService(IServiceScopeFactory factory, ILogger<MigratorHostedService<TDbContext>> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        using var scope = _factory.CreateScope();

        var database = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var pending = await database.Database.GetPendingMigrationsAsync(cancellationToken);
        var total = pending.Count();

        if (total <= 0)
        {
            _logger.LogInformation("no pending migrations for database context {0}.", typeof(TDbContext).Name);
            return;
        }

        _logger.LogInformation("applying {0} pending migrations for database context {1}...", total, typeof(TDbContext).Name);
        stopwatch.Start();
        await database.Database.MigrateAsync(cancellationToken);
        stopwatch.Stop();
        _logger.LogInformation("successfully applied {0} migrations for database context {1} in {2} ms.", total, typeof(TDbContext).Name, stopwatch.ElapsedMilliseconds);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}