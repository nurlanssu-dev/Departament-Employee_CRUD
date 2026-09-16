using Microsoft.EntityFrameworkCore;
using MyProject.Business.Services.Interfaces;
using MyProject.DataAccess.Contexts;
using MyProject.Entity.Entities;

namespace MyProject.Business.Services.Implementation;

public class EmployeeService : IEmployeeService
{
    private readonly MyProjectContext _context;

    public EmployeeService(MyProjectContext context)
    {
        _context = context;
    }

    // CREATE
    public async Task<Employee?> CreateAsync(Employee employee)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == employee.DepartmentId);

        if (department is null)
            return null;

        var currentCount = await _context.Employees
            .CountAsync(e => e.DepartmentId == employee.DepartmentId);

        if (department.Limit > 0 && currentCount >= department.Limit)
            return null; // department is full

        employee.CreatedAt = DateTime.UtcNow;
        employee.UpdatedAt = null;

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        return employee;
    }

    // GET ALL
    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .ToListAsync();
    }

    // GET BY ID
    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // UPDATE
    public async Task<bool> UpdateAsync(int id, Employee employee)
    {
        var existing = await _context.Employees.FindAsync(id);

        if (existing is null)
            return false;

        // If department is changing, ensure target department has capacity
        if (existing.DepartmentId != employee.DepartmentId)
        {
            var targetDept = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == employee.DepartmentId);

            if (targetDept is null)
                return false;

            var targetCount = await _context.Employees
                .CountAsync(e => e.DepartmentId == employee.DepartmentId && e.Id != id);

            if (targetDept.Limit > 0 && targetCount >= targetDept.Limit)
                return false; // cannot move, target full
        }

        existing.FirstName = employee.FirstName;
        existing.LastName = employee.LastName;
        existing.Email = employee.Email;
        existing.PhoneNumber = employee.PhoneNumber;
        existing.DateOfBirth = employee.DateOfBirth;
        existing.DepartmentId = employee.DepartmentId;


        await _context.SaveChangesAsync();

        return true;
    }

    // DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
            return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return true;
    }
}
