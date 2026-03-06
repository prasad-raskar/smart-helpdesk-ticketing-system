using Helpdesk.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Helpdesk.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<GenericRepository<T>> _logger;

    public GenericRepository(ApplicationDbContext context, ILogger<GenericRepository<T>> logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        _logger.LogInformation("[INFO] Fetching {Entity} with ID: {Id}", typeof(T).Name, id);
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        _logger.LogInformation("[INFO] Fetching all {Entity} records", typeof(T).Name);
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        _logger.LogInformation("[INFO] Searching {Entity} records with criteria", typeof(T).Name);
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        _logger.LogInformation("[INFO] Adding new {Entity}", typeof(T).Name);
        await _dbSet.AddAsync(entity);
    }

    public virtual Task UpdateAsync(T entity)
    {
        _logger.LogInformation("[INFO] Updating {Entity}", typeof(T).Name);
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _logger.LogInformation("[INFO] Deleting {Entity} with ID: {Id}", typeof(T).Name, id);
            _dbSet.Remove(entity);
        }
        else
        {
            _logger.LogWarning("[WARN] Attempted to delete {Entity} with ID: {Id}, but it was not found", typeof(T).Name, id);
        }
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            _logger.LogInformation("[INFO] Persisting changes to database");
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ERROR] Database operation failed during SaveChanges");
            throw;
        }
    }
}
