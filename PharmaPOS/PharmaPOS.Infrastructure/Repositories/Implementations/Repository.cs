using Microsoft.EntityFrameworkCore;
using PharmaPOS.Infrastructure.Data;
using PharmaPOS.Infrastructure.Repositories.Interfaces;

namespace PharmaPOS.Infrastructure.Repositories.Implementations;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly PharmaPOSDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(PharmaPOSDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.FindAsync(id) is not null;
    }
}