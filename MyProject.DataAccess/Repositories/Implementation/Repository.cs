using Microsoft.EntityFrameworkCore;
using MyProject.DataAccess.Contexts;
using MyProject.DataAccess.Repositories.Interfaces;
using MyProject.Entity.Entities.Common;

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

    public void Delete(T entity)
    {
        Table.Remove(entity);
    }

    public IQueryable<T> GetAll()
    {
        return Table.AsQueryable();
    }

    public Task<T?> GetByIdAsync(int id)
    {
        return Table.FirstOrDefaultAsync(e => e.Id == id);
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
