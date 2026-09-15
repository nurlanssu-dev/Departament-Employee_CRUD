namespace MyProject.Entity.Entities.Common;

public class AuditEntity : BasaEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
