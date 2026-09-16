using MyProject.Entity.Entities;

namespace MyProject.Business.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee?> CreateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<List<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, Employee employee);
    }
}