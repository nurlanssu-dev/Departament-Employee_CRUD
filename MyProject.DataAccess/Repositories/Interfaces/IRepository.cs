using MyProject.Entity.Entities.Common;
using System.Linq.Expressions;

namespace MyProject.DataAccess.Repositories.Interfaces;

public interface IRepository<T> where T : BasaEntity
{
    public IQueryable<T> GetAll();
    public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate=null);
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}
