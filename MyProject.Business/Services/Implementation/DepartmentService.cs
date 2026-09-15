using MyProject.DataAccess.Contexts;
using MyProject.Entity.Entities;

namespace MyProject.Business.Services.Implementation;

public class DepartmentService
{
    private readonly MyProjectContext _context;

    public DepartmentService(MyProjectContext context)
    {
        _context = context;
    }

    // CREATE
    public async Task<Department> CreateAsync(Department department)
    {
        department.CreatedAt = DateTime.UtcNow;
        department.UpdatedAt = null;

        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();

        return department;
    }
}
