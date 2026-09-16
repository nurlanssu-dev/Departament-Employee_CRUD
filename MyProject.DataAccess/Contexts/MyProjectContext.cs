using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyProject.DataAccess.Configuration;
using MyProject.Entity.Entities;
using MyProject.Entity.Entities.Common;

namespace MyProject.DataAccess.Contexts;

public class MyProjectContext : DbContext
{
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=MyProjectDb;Trusted_Connection=True;TrustServerCertificate=True;");
        base.OnConfiguring(optionsBuilder);
    }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DepartamentConfiguration());
        base.OnModelCreating(modelBuilder);
    }


    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,CancellationToken cancellationToken = default)
    {
        var datas = ChangeTracker.Entries<AuditEntity>().ToList();

        foreach (EntityEntry<AuditEntity> data in datas)
        {
            switch (data.State)
            {
                case EntityState.Added:
                    data.Entity.CreatedAt = DateTime.Now;
                    break;

                case EntityState.Modified:
                    data.Entity.UpdatedAt = DateTime.Now;
                    break;

                default:
                    break;
            }
        }

        return base.SaveChangesAsync( acceptAllChangesOnSuccess, cancellationToken);
    }


}
