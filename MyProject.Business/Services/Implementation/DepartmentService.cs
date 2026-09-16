using Microsoft.EntityFrameworkCore;
using MyProject.Business.Services.Interfaces;
using MyProject.DataAccess.Repositories.Interfaces;
using MyProject.Entity.Entities;

namespace MyProject.Business.Services.Implementation;

public class DepartmentService : IDepartmentService
{
    private readonly IRepository<Department> _repository;

    public DepartmentService(IRepository<Department> repository)
    {
        _repository = repository;
    }

    // CREATE
    public async Task<Department> CreateAsync(Department department)
    {
        var exists = await _repository.AnyAsync(
            d => d.Name == department.Name);

        if (exists)
        {
            throw new InvalidOperationException(
                "A department with this name already exists.");
        }

        await _repository.AddAsync(department);
        await _repository.SaveChangesAsync();

        return department;
    }

    // GET ALL
    public List<Department> GetAll()
    {
        return _repository.GetAll(
            null,
            d => d.OrderBy(d => d.Name),
            d => d.Include(d => d.Employees)
                  .ThenInclude(e => e.EmployeeProjects)
                  .ThenInclude(ep => ep.Project)
        ).ToList();
    }

    // GET BY ID
    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    // UPDATE
    public async Task<bool> UpdateAsync(
        int id,
        Department department)
    {
        var existingDepartment =
            await _repository.GetByIdAsync(id);

        if (existingDepartment is null)
            return false;

        var nameExists = await _repository.AnyAsync(d =>
            d.Name == department.Name &&
            d.Id != id);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "A department with this name already exists.");
        }

        existingDepartment.Name = department.Name;
        existingDepartment.Description = department.Description;
        existingDepartment.Limit = department.Limit;
        existingDepartment.Location = department.Location;

        _repository.Update(existingDepartment);

        await _repository.SaveChangesAsync();

        return true;
    }

    // DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var department =
            await _repository.GetByIdAsync(id);

        if (department is null)
            return false;

        _repository.Delete(department);

        await _repository.SaveChangesAsync();

        return true;
    }
}