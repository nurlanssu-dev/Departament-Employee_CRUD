# Department, Employee & Project Management Application

This project is a simple **Department, Employee, and Project Management System** developed with **C#**, **.NET**, **Entity Framework Core**, and **SQL Server**.

The project demonstrates CRUD operations, entity relationships, layered architecture, generic repository usage, Entity Framework Core migrations, Fluent API configurations, LINQ queries, eager loading, and automatic audit tracking.

---

## Technologies

- C#
- .NET
- Entity Framework Core
- SQL Server
- LINQ
- Async / Await
- Fluent API
- EF Core Migrations
- Repository Pattern

---

## Project Structure

The solution is divided into four main layers:

```text
src
│
├── MyProject.Entity
│   └── Entities
│       ├── Common
│       │   ├── BasaEntity.cs
│       │   └── AuditEntity.cs
│       │
│       ├── Enums
│       │   └── EployeeStatus.cs
│       │
│       ├── Department.cs
│       ├── Employee.cs
│       ├── Project.cs
│       └── EmployeeProject.cs
│
├── MyProject.DataAccess
│   ├── Configuration
│   │   ├── DepartmentConfiguration.cs
│   │   ├── EmployeeConfiguration.cs
│   │   └── EmployeeProjectConfiguration.cs
│   │
│   ├── Contexts
│   │   └── MyProjectContext.cs
│   │
│   ├── Repositories
│   │   ├── Interfaces
│   │   │   ├── IRepository.cs
│   │   │   └── IDepartmentRepository.cs
│   │   │
│   │   └── Implementation
│   │       ├── Repository.cs
│   │       └── DepartmentRepository.cs
│   │
│   └── Migrations
│
├── MyProject.Business
│   └── Services
│       ├── Interfaces
│       │   ├── IDepartmentService.cs
│       │   └── IEmployeeService.cs
│       │
│       └── Implementation
│           ├── DepartmentService.cs
│           └── EmployeeService.cs
│
└── MyProject.Presentation
    └── Program.cs
```

---

# Entities

## Department

A department contains basic department information and a collection of employees.

```csharp
public class Department : AuditEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Limit { get; set; }
    public string? Location { get; set; }

    public List<Employee>? Employees { get; set; } = new List<Employee>();
}
```

### Department properties

| Property | Description |
|---|---|
| Id | Unique identifier |
| Name | Department name |
| Description | Department description |
| Limit | Maximum number of employees |
| Location | Department location |
| Employees | Employees that belong to the department |
| CreatedAt | Creation date |
| UpdatedAt | Last update date |

---

## Employee

Each employee belongs to one department and can be assigned to multiple projects.

```csharp
public class Employee : AuditEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    public int DepartmentId { get; set; }
    public EployeeStatus Status { get; set; } = EployeeStatus.Active;

    public Department Department { get; set; } = null!;
    public List<EmployeeProject>? EmployeeProjects { get; set; }
}
```

### Employee properties

| Property | Description |
|---|---|
| Id | Unique identifier |
| FirstName | Employee first name |
| LastName | Employee last name |
| Email | Employee email |
| PhoneNumber | Employee phone number |
| DateOfBirth | Employee birth date |
| DepartmentId | Foreign key of Department |
| Status | Employee status |
| Department | Related Department navigation property |
| EmployeeProjects | Employee-Project relationship records |
| CreatedAt | Creation date |
| UpdatedAt | Last update date |

---

## Project

A project contains project information and can have multiple employees assigned to it.

```csharp
public class Project : AuditEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<EmployeeProject>? EmployeeProjects { get; set; }
}
```

### Project properties

| Property | Description |
|---|---|
| Id | Unique identifier |
| Name | Project name |
| Description | Project description |
| EmployeeProjects | Employee-Project relationship records |
| CreatedAt | Creation date |
| UpdatedAt | Last update date |

---

## EmployeeProject

`EmployeeProject` is a join entity used to represent the many-to-many relationship between `Employee` and `Project`.

```csharp
public class EmployeeProject : BasaEntity
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
```

Although `EmployeeProject` inherits from `BasaEntity`, its inherited `Id` property is ignored in the EF Core configuration because the table uses a composite key.

---

# Entity Relationships

The project contains two main relationship types.

## Department - Employee: One-to-Many

```text
Department
    1
    |
    |
    *
Employee
```

One department can contain multiple employees, while each employee belongs to one department.

---

## Employee - Project: Many-to-Many

The many-to-many relationship is implemented explicitly through the `EmployeeProject` join table.

```text
Employee
   1
   |
   *
EmployeeProject
   *
   |
   1
Project
```

From the application point of view:

```text
Employee * -------- * Project
```

An employee can participate in multiple projects, and a project can contain multiple employees.

---

# EmployeeProject Configuration

The join table uses a composite primary key consisting of `EmployeeId` and `ProjectId`.

```csharp
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
```

This configuration ensures that the same employee-project pair cannot be inserted more than once.

Example:

```text
EmployeeId | ProjectId
-----------|----------
1          | 1
1          | 2
2          | 1
```

The pair `(1, 1)` cannot be inserted again because it is already part of the composite primary key.

---

# Audit System

`Department`, `Employee`, and `Project` inherit from `AuditEntity`.

```csharp
public class AuditEntity : BasaEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

`CreatedAt` and `UpdatedAt` values are automatically managed inside `MyProjectContext`.

```csharp
public override Task<int> SaveChangesAsync(
    bool acceptAllChangesOnSuccess,
    CancellationToken cancellationToken = default)
{
    var datas = ChangeTracker
        .Entries<AuditEntity>()
        .ToList();

    foreach (var data in datas)
    {
        switch (data.State)
        {
            case EntityState.Added:
                data.Entity.CreatedAt = DateTime.Now;
                break;

            case EntityState.Modified:
                data.Entity.UpdatedAt = DateTime.Now;
                break;
        }
    }

    return base.SaveChangesAsync(
        acceptAllChangesOnSuccess,
        cancellationToken);
}
```

Because of this implementation, audit dates do not need to be manually assigned inside services.

---

# Generic Repository

The project uses a generic repository to centralize common database operations.

```csharp
public class Repository<T> : IRepository<T> where T : BasaEntity
{
    private readonly MyProjectContext _context;
    private readonly DbSet<T> Table;

    public Repository(MyProjectContext context)
    {
        _context = context;
        Table = _context.Set<T>();
    }
}
```

### Repository operations

- `AddAsync()`
- `AnyAsync()`
- `GetAll()`
- `GetByIdAsync()`
- `Update()`
- `Delete()`
- `SaveChangesAsync()`

The advanced `GetAll()` overload supports:

- Filtering with `Expression<Func<T, bool>>`
- Ordering
- Eager loading with `Include()` / `ThenInclude()`
- Optional `AsNoTracking()` behavior

Example:

```csharp
public IQueryable<T> GetAll(
    Expression<Func<T, bool>>? predicate = null,
    Func<IQueryable<T>, IQueryable<T>>? orderBy = null,
    Func<IQueryable<T>, IQueryable<T>>? include = null,
    bool disableTracking = true)
{
    var query = Table.AsQueryable();

    if (disableTracking)
        query = query.AsNoTracking();

    if (include != null)
        query = include(query);

    if (predicate != null)
        query = query.Where(predicate);

    if (orderBy != null)
        query = orderBy(query);

    return query;
}
```

---

# Department Service

`DepartmentService` contains CRUD operations and business rules for departments.

### Available operations

```text
CreateAsync()
GetAll()
GetByIdAsync()
UpdateAsync()
DeleteAsync()
```

### Business Rules

When creating a department:

- A department with the same name cannot already exist.

When updating a department:

- The department must exist.
- Another department cannot already have the new department name.

The service uses `IRepository<Department>` instead of accessing `MyProjectContext` directly.

```csharp
private readonly IRepository<Department> _repository;

public DepartmentService(IRepository<Department> repository)
{
    _repository = repository;
}
```

### Loading employees and their projects

Department queries can eagerly load employees and each employee's projects:

```csharp
public List<Department> GetAll()
{
    return _repository.GetAll(
        null,
        d => d.OrderBy(d => d.Name),
        d => d.Include(d => d.Employees)
              .ThenInclude(e => e.EmployeeProjects)
              .ThenInclude(ep => ep.Project)
    ).ToList();
}
```

This returns departments together with their employees and the projects assigned to those employees.

---

# Employee Service

`EmployeeService` contains CRUD operations for employees.

### Available operations

```text
CreateAsync()
GetAllAsync()
GetByIdAsync()
UpdateAsync()
DeleteAsync()
```

## Employee Business Rules

Before creating an employee, the system checks whether the selected department exists.

If the department does not exist, the employee is not created.

### Department Employee Limit

Each department has a maximum employee limit.

Before an employee is added, the number of existing employees in the department is checked.

This prevents adding employees to a department that has already reached its maximum capacity.

### Changing Employee Department

When an employee is moved to another department, the system checks:

1. Whether the target department exists.
2. Whether the target department has enough capacity.

Only after these checks is the employee's `DepartmentId` updated.

---

# Entity Framework Core

The application uses `MyProjectContext` as the Entity Framework Core database context.

```csharp
public class MyProjectContext : DbContext
{
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<EmployeeProject> EmployeeProjects { get; set; } = null!;
}
```

All entity configurations are loaded automatically from the DataAccess assembly:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyProjectContext).Assembly);
    base.OnModelCreating(modelBuilder);
}
```

SQL Server is used as the database provider.

Example connection:

```csharp
optionsBuilder.UseSqlServer(
    "Server=localhost;" +
    "Database=MyProjectDb;" +
    "Trusted_Connection=True;" +
    "TrustServerCertificate=True;");
```

---

# Database Tables

The project currently creates the following application tables:

```text
Departments
Employees
Projects
EmployeeProjects
```

Entity Framework Core also creates:

```text
__EFMigrationsHistory
```

which is used to keep track of applied migrations.

### EmployeeProjects table

The join table contains the two foreign keys that also form its composite primary key:

```text
EmployeeProjects
-------------------------
EmployeeId   PK, FK
ProjectId    PK, FK
```

---

# Migrations

Create a new migration:

```powershell
Add-Migration MigrationName
```

Example:

```powershell
Add-Migration mig_4
```

Apply migrations to the database:

```powershell
Update-Database
```

Remove the latest migration before applying it:

```powershell
Remove-Migration
```

The project migrations create the required tables, foreign keys, one-to-many relationship, and the many-to-many relationship through `EmployeeProjects`.

---

# Running the Application

The Presentation layer can create the context, repositories, and services.

Example for `DepartmentService`:

```csharp
using MyProject.Business.Services.Implementation;
using MyProject.DataAccess.Contexts;
using MyProject.DataAccess.Repositories.Implementation;
using MyProject.DataAccess.Repositories.Interfaces;
using MyProject.Entity.Entities;

using var context = new MyProjectContext();

IRepository<Department> departmentRepository =
    new Repository<Department>(context);

var departmentService =
    new DepartmentService(departmentRepository);
```

The service methods can then be called from `Program.cs`.

---

## Get All Departments with Employees and Projects

```csharp
var departments = departmentService.GetAll();

foreach (var department in departments)
{
    Console.WriteLine(
        $"{department.Id} | " +
        $"{department.Name} | " +
        $"{department.Location} | " +
        $"Limit: {department.Limit}");

    if (department.Employees is null)
        continue;

    foreach (var employee in department.Employees)
    {
        Console.WriteLine(
            $"  Employee: {employee.FirstName} {employee.LastName}");

        if (employee.EmployeeProjects is null)
            continue;

        foreach (var employeeProject in employee.EmployeeProjects)
        {
            Console.WriteLine(
                $"    Project: {employeeProject.Project.Name}");
        }
    }
}
```

---

## Create Department

```csharp
var newDepartment = new Department
{
    Name = "Logistics",
    Description = "Logistics department",
    Limit = 20,
    Location = "Floor 2"
};

var createdDepartment =
    await departmentService.CreateAsync(newDepartment);
```

---

## Example Project Data

```sql
INSERT INTO Projects (Name, Description, CreatedAt)
VALUES
('CRM System', 'Customer relationship management system', GETDATE()),
('HR Management', 'Employee and department management system', GETDATE()),
('E-Commerce', 'Online sales platform', GETDATE()),
('Mobile Application', 'Company mobile application', GETDATE()),
('Reporting System', 'Reporting and analytics system', GETDATE());
```

Example employee-project assignments:

```sql
INSERT INTO EmployeeProjects (EmployeeId, ProjectId)
VALUES
(1, 1),
(1, 2),
(2, 1),
(2, 3),
(3, 2),
(3, 4);
```

The employee IDs must already exist in the `Employees` table and the project IDs must already exist in the `Projects` table.

---

# CRUD Operations

CRUD operations are currently implemented for the main management entities.

| Operation | Department | Employee |
|---|---:|---:|
| Create | ✅ | ✅ |
| Get All | ✅ | ✅ |
| Get By Id | ✅ | ✅ |
| Update | ✅ | ✅ |
| Delete | ✅ | ✅ |

`Project` and `EmployeeProject` have been added to the data model and database relationship structure. `EmployeeProject` is used as the explicit join table between employees and projects.

---

# Features

- Layered project architecture
- Department CRUD operations
- Employee CRUD operations
- SQL Server database
- Entity Framework Core
- Async database operations
- EF Core migrations
- Fluent API configurations
- Generic repository pattern
- One-to-many `Department -> Employee` relationship
- Many-to-many `Employee <-> Project` relationship
- Explicit `EmployeeProject` join table
- Composite primary key on `EmployeeProject`
- Department employee capacity control
- Department existence validation
- Duplicate department name validation
- Employee status support
- Automatic `CreatedAt` tracking
- Automatic `UpdatedAt` tracking
- `AsNoTracking()` support for read operations
- Filtering and ordering through the generic repository
- Related data loading with `Include()` and `ThenInclude()`
- Automatic configuration discovery with `ApplyConfigurationsFromAssembly()`

---

# Project Purpose

The main purpose of this project is to practice:

- C# OOP principles
- Layered architecture
- Repository pattern
- Entity Framework Core
- Database relationships
- One-to-many relationships
- Many-to-many relationships
- Join tables and composite keys
- CRUD operations
- Service layer implementation
- Interfaces
- Async / Await
- LINQ
- SQL Server
- Migrations
- Fluent API
- Eager loading
- Business rule validation
- Entity audit tracking

---

## Author

Developed as a practical C# and Entity Framework Core CRUD project.
