using MyProject.Entity.Entities.Common;

namespace MyProject.Entity.Entities;

public class EmployeeProject : BasaEntity
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
