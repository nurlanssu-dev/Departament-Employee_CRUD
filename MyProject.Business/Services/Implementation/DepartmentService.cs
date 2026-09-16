using Microsoft.EntityFrameworkCore;
using MyProject.Business.Services.Interfaces;
using MyProject.DataAccess.Contexts;
using MyProject.Entity.Entities;

namespace MyProject.Business.Services.Implementation;

public class DepartmentService : IDepartmentService
{
    private readonly MyProjectContext _context;

    public DepartmentService(MyProjectContext context)
    {
        _context = context;
    }

    // CREATE
    public async Task<Department> CreateAsync(Department department)
    {
        var exists = await _context.Departments
            .AnyAsync(d => d.Name == department.Name);

        if (exists)
        {
            throw new InvalidOperationException(
                "A department with this name already exists.");
        }

        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();

        return department;
    }

    // GET ALL
    public async Task<List<Department>> GetAllAsync()
    {
        return await _context.Departments
            .Include(d => d.Employees)
            .ThenInclude(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project)
            .AsNoTracking()
            .ToListAsync();
    }

    // GET BY ID
    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    // UPDATE
    public async Task<bool> UpdateAsync(int id, Department department)
    {
        var existingDepartment =
            await _context.Departments.FindAsync(id);

        if (existingDepartment is null)
            return false;

        var nameExists = await _context.Departments
            .AnyAsync(d =>
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

        await _context.SaveChangesAsync();

        return true;
    }

    // DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var department =
            await _context.Departments.FindAsync(id);

        if (department is null)
            return false;

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();

        return true;
    }
}
