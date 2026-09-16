using MyProject.Business.Services.Implementation;
using MyProject.DataAccess.Contexts;
using MyProject.Entity.Entities;

using var context = new MyProjectContext();

var departmentService = new DepartmentService(context);
var employeeService = new EmployeeService(context);

Console.WriteLine("Program started");


//// DEPARTMENTS
//Console.WriteLine("\nDEPARTMENTS");

//var departments = await departmentService.GetAllAsync();

//foreach (var department in departments)
//{
//    Console.WriteLine(
//        $"{department.Id} | " +
//        $"{department.Name} | " +
//        $"{department.Location} | " +
//        $"Limit: {department.Limit}");
//}


//// EMPLOYEES
//Console.WriteLine("\nEMPLOYEES");

//var employees = await employeeService.GetAllAsync();

//foreach (var employee in employees)
//{
//    Console.WriteLine(
//        $"{employee.Id} | " +
//        $"{employee.FirstName} {employee.LastName} | " +
//        $"{employee.Email} | " +
//        $"Department: {employee.Department.Name}");
//}


//GetByIdAsync(department)

//var department = await departmentService.GetByIdAsync(100);

//if (department is not null)
//{
//    Console.WriteLine($"{department.Id} | {department.Name} | {department.Location} | Limit: {department.Limit}");
//}
//else
//{
//    Console.WriteLine("Department not found.");
//}

//GetByIdAsync(employee)

//var employee = await employeeService.GetByIdAsync(10);

//if (employee is not null)
//{
//    Console.WriteLine($"{employee.Id} | {employee.FirstName} {employee.LastName} | {employee.Email} | Department: {employee.Department.Name}");
//}
//else
//{
//    Console.WriteLine("Employee not found.");
//}

//CreateAsync(department)

//var newDepartment = new Department
//{
//    Name = "Logistics",
//    Description = "Logistics department",
//    Limit = 20,
//    Location = "Floor 2"
//};

//var createdDepartment = await departmentService.CreateAsync(newDepartment);

//Console.WriteLine($"Created department: {createdDepartment.Id} | {createdDepartment.Name} | {createdDepartment.Description} | Limit: {createdDepartment.Limit} | Location: {createdDepartment.Location}");


//CreateAsync(employee)

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
        "Employee yaradılmadı. Department yoxdur və ya limit doludur.");
}
else
{
    Console.WriteLine(
        $"Created: {createdEmployee.Id} - " +
        $"{createdEmployee.FirstName} {createdEmployee.LastName}");
}
