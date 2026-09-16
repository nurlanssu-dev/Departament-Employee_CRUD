using Microsoft.EntityFrameworkCore.Query;
using MyProject.Entity.Entities.Common;
using System.Linq.Expressions;

namespace MyProject.DataAccess.Repositories.Interfaces;

public interface IRepository<T> where T : BasaEntity
{
    public IQueryable<T> GetAll();
    public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate=null, Func<IQueryable<T>, IQueryable<T>> orderBy=null, Func<IQueryable<T>, IQueryable<T>>? include=null, bool disableTracking=true);
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(int id, bool disableTracking=true, Func<IQueryable<T>, IQueryable<T>>? include=null);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}
