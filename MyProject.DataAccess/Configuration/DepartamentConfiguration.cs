using Microsoft.EntityFrameworkCore;
using MyProject.Entity.Entities;

namespace MyProject.DataAccess.Configuration;

internal class DepartamentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Description).IsRequired(false).HasMaxLength(500);
        builder.Property(d => d.Limit).IsRequired();
        builder.Property(d => d.Location).IsRequired(false).HasMaxLength(200);
        builder.Property(d => d.CreatedAt).IsRequired();
        builder.Property(d => d.UpdatedAt).IsRequired(false);

    }
}
