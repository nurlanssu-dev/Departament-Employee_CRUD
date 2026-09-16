using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Entity.Entities;

namespace MyProject.DataAccess.Configuration;

public class EmployeeProjectConfiguration : IEntityTypeConfiguration<EmployeeProject>
{
    public void Configure(EntityTypeBuilder<EmployeeProject> builder)
    {
        builder.HasKey(ep => new { ep.EmployeeId, ep.ProjectId });
        builder.HasOne(ep => ep.Employee)
               .WithMany(e => e.EmployeeProjects)
               .HasForeignKey(ep => ep.EmployeeId);
        builder.HasOne(ep => ep.Project)
               .WithMany(p => p.EmployeeProjects)
               .HasForeignKey(ep => ep.ProjectId);
        builder.Ignore(ep => ep.Id);
    }
}
