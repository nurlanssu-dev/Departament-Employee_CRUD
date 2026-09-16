# Department & Employee CRUD Application

This project is a simple **Department and Employee Management System** developed with **C#**, **.NET**, **Entity Framework Core**, and **SQL Server**.

The project demonstrates CRUD operations, entity relationships, layered architecture, Entity Framework Core migrations, Fluent API configurations, and automatic audit tracking.

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

---

## Project Structure

The solution is divided into four main layers:

```text
src
│
├── MyProject.Entity
│   └── Entities
│       ├── Common
│       │   ├── BaseEntity.cs
│       │   └── AuditEntity.cs
│       │
│       ├── Department.cs
│       └── Employee.cs
│
├── MyProject.DataAccess
│   ├── Configuration
│   │   ├── DepartamentConfiguration.cs
│   │   └── EmployeeConfiguration.cs
│   │
│   ├── Contexts
│   │   └── MyProjectContext.cs
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
| CreatedAt | Creation date |
| UpdatedAt | Last update date |

---

## Employee

Each employee belongs to one department.

```csharp
public class Employee : AuditEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
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
| CreatedAt | Creation date |
| UpdatedAt | Last update date |

---

# Entity Relationship

The project contains a **one-to-many relationship** between `Department` and `Employee`.

```text
Department
    1
    |
    |
    *
Employee
```

One department can contain multiple employees, while each employee belongs to only one department.

---

# Audit System

Both `Department` and `Employee` inherit from `AuditEntity`.

```csharp
public class AuditEntity : BaseEntity
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

# Department Service

`DepartmentService` contains CRUD operations for departments.

### Available operations

```text
CreateAsync()
GetAllAsync()
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

Example:

```csharp
var exists = await _context.Departments
    .AnyAsync(d => d.Name == department.Name);

if (exists)
{
    throw new InvalidOperationException(
        "A department with this name already exists.");
}
```

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

---

## Employee Business Rules

Before creating an employee, the system checks whether the selected department exists.

```csharp
var department = await _context.Departments
    .AsNoTracking()
    .FirstOrDefaultAsync(d => d.Id == employee.DepartmentId);
```

If the department does not exist, the employee is not created.

---

### Department Employee Limit

Each department has a maximum employee limit.

Before an employee is added, the number of existing employees in the department is checked.

```csharp
var currentCount = await _context.Employees
    .CountAsync(e => e.DepartmentId == employee.DepartmentId);

if (department.Limit > 0 &&
    currentCount >= department.Limit)
{
    return null;
}
```

This prevents adding employees to a department that has reached its maximum capacity.

---

## Changing Employee Department

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

The project creates two main database tables:

```text
Departments
Employees
```

Entity Framework Core also creates:

```text
__EFMigrationsHistory
```

to keep track of applied migrations.

---

# Migrations

Create a new migration:

```powershell
Add-Migration InitialCreate
```

Apply migrations to the database:

```powershell
Update-Database
```

---

# Running the Application

The Presentation layer creates instances of the context and services.

```csharp
using MyProject.Business.Services.Implementation;
using MyProject.DataAccess.Contexts;

using var context = new MyProjectContext();

var departmentService = new DepartmentService(context);
var employeeService = new EmployeeService(context);
```

The service methods can then be called from `Program.cs`.

---

## Get All Departments

```csharp
var departments = await departmentService.GetAllAsync();

foreach (var department in departments)
{
    Console.WriteLine(
        $"{department.Id} | " +
        $"{department.Name} | " +
        $"{department.Location} | " +
        $"Limit: {department.Limit}");
}
```

---

## Get All Employees

```csharp
var employees = await employeeService.GetAllAsync();

foreach (var employee in employees)
{
    Console.WriteLine(
        $"{employee.Id} | " +
        $"{employee.FirstName} {employee.LastName} | " +
        $"{employee.Email} | " +
        $"Department: {employee.Department.Name}");
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

## Create Employee

```csharp
var newEmployee = new Employee
{
    FirstName = "Nurlan",
    LastName = "Aliyev",
    Email = "nurlan.aliyev@gmail.com",
    PhoneNumber = "+994501234567",
    DateOfBirth = new DateTime(2000, 5, 20),
    DepartmentId = 2
};

var createdEmployee =
    await employeeService.CreateAsync(newEmployee);

if (createdEmployee is null)
{
    Console.WriteLine(
        "Employee could not be created. " +
        "Department does not exist or department limit has been reached.");
}
else
{
    Console.WriteLine(
        $"Employee created: " +
        $"{createdEmployee.FirstName} " +
        $"{createdEmployee.LastName}");
}
```

---

# CRUD Operations

The following CRUD operations are implemented for both entities:

| Operation | Department | Employee |
|---|:---:|:---:|
| Create | ✅ | ✅ |
| Get All | ✅ | ✅ |
| Get By Id | ✅ | ✅ |
| Update | ✅ | ✅ |
| Delete | ✅ | ✅ |

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
- One-to-many relationship
- Department employee capacity control
- Department existence validation
- Duplicate department name validation
- Automatic `CreatedAt` tracking
- Automatic `UpdatedAt` tracking
- `AsNoTracking()` for read operations
- Related Department data loaded with `Include()`

---

# Project Purpose

The main purpose of this project is to practice:

- C# OOP principles
- Layered architecture
- Entity Framework Core
- Database relationships
- CRUD operations
- Service layer implementation
- Interfaces
- Async / Await
- LINQ
- SQL Server
- Migrations
- Business rule validation
- Entity audit tracking

---

## Author

Developed as a practical C# and Entity Framework Core CRUD project.
