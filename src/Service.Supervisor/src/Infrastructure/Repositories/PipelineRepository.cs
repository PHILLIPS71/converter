using System.Linq.Expressions;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class PipelineRepository : IPipelineRepository
{
    private readonly ApplicationDbContext _database;

    public PipelineRepository(ApplicationDbContext database)
    {
        _database = database;
    }

    public IQueryable<Pipeline> ToQueryable()
    {
        return _database
            .Pipelines
            .AsQueryable();
    }

    public Task<bool> ExistsAsync(
        Expression<Func<Pipeline, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().AnyAsync(predicate, cancellation);
    }

    public Task<Pipeline> FirstAsync(
        Expression<Func<Pipeline, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().FirstAsync(predicate, cancellation);
    }

    public Task<Pipeline?> FirstOrDefaultAsync(
        Expression<Func<Pipeline, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().FirstOrDefaultAsync(predicate, cancellation);
    }

    public Task<Pipeline> SingleAsync(
        Expression<Func<Pipeline, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleAsync(predicate, cancellation);
    }

    public Task<Pipeline?> SingleOrDefaultAsync(
        Expression<Func<Pipeline, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleOrDefaultAsync(predicate, cancellation);
    }

    public Task<List<Pipeline>> ToListAsync(
        Expression<Func<Pipeline, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().Where(predicate).ToListAsync(cancellation);
    }

    public Pipeline Create(Pipeline entity)
    {
        return _database.Pipelines.Add(entity).Entity;
    }

    public Pipeline Update(Pipeline entity)
    {
        return _database.Pipelines.Update(entity).Entity;
    }

    public Pipeline Delete(Pipeline entity)
    {
        return _database.Pipelines.Remove(entity).Entity;
    }

    public async Task<Pipeline?> GetByPipelineExecutionIdAsync(Guid id, CancellationToken cancellation = default)
    {
        return await _database
            .PipelineExecutions
            .Where(x => x.Id == id)
            .Include(x => x.Pipeline.Executions.Where(e => e.Id == id))
            .Select(x => x.Pipeline)
            .FirstOrDefaultAsync(cancellation);
    }
}