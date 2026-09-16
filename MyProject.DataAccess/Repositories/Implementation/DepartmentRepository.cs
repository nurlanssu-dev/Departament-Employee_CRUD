using MyProject.DataAccess.Contexts;
using MyProject.DataAccess.Repositories.Interfaces;
using MyProject.Entity.Entities;

namespace MyProject.DataAccess.Repositories.Implementation;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(MyProjectContext context) : base(context)
    {
    }
}
