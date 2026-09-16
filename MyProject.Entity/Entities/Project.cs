using MyProject.Entity.Entities.Common;

namespace MyProject.Entity.Entities;

public class Project : AuditEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<EmployeeProject>? EmployeeProjects { get; set; }
}
