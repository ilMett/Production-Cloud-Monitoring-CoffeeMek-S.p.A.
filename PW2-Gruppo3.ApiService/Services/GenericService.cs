using Microsoft.EntityFrameworkCore;
using PW2_Gruppo3.ApiService.Data;

namespace PW2_Gruppo3.ApiService.Services;

public interface IGenericService<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task UpdateAsync(T entity);
    Task InsertAsync(T entity);
    Task DeleteAsync(Guid id);
}

public class GenericService<T> : IGenericService<T> where T : class
{
    private readonly ProductionMonitoringContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericService(ProductionMonitoringContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task InsertAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
