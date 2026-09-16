using Microsoft.EntityFrameworkCore;
using MyProject.DataAccess.Contexts;
using MyProject.DataAccess.Repositories.Interfaces;
using MyProject.Entity.Entities.Common;
using System.Linq.Expressions;

namespace MyProject.DataAccess.Repositories.Implementation;

public class Repository<T> : IRepository<T> where T : BasaEntity
{
    private readonly MyProjectContext _context;
    private readonly DbSet<T> Table;

    public Repository(MyProjectContext context)
    {
        _context = context;
        Table = _context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await Table.AddAsync(entity);
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return Table.AnyAsync(predicate);
    }

    public void Delete(T entity)
    {
        Table.Remove(entity);
    }

    public IQueryable<T> GetAll()
    {
        return Table.AsQueryable();
    }

    public IQueryable<T> GetAll(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool disableTracking = true)
    {
        var query = Table.AsQueryable();

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }

    public Task<T?> GetByIdAsync(int id)
    {
        return Table.FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<T?> GetByIdAsync(
        int id,
        bool disableTracking = true,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        var query = Table.AsQueryable();

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        return query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Update(T entity)
    {
        Table.Update(entity);
    }
}