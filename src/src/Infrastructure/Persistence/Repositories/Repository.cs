using Microsoft.EntityFrameworkCore;
using QuizApp.Application.Interfaces;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(int id,CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(
            new object[] { id },
            cancellationToken);
    }

    public void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}