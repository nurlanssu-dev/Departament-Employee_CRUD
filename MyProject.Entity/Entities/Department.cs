using MyProject.Entity.Entities.Common;

namespace MyProject.Entity.Entities;

public class Department : AuditEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Limit { get; set; }
    public string? Location { get; set; }
}
