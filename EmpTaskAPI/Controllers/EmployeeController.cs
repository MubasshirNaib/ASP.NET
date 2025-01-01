using System.Security.Claims;
using EmpTaskAPI.DataAccessLayer;
using EmpTaskAPI.DTOModels;
using EmpTaskAPI.HashPassword;
using EmpTaskAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmpTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDBContext context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger, AppDBContext context)
        {
            _logger = logger;
            this.context = context;
        }

        /// <summary>
        /// This EndPoint gives the list of all employees
        /// </summary>

        /// <returns></returns>

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> GetEmployees()
        {
            _logger.LogInformation("GetEmployees endpoint called.");
            var data = await context.Employees.ToListAsync();

            if (data == null || data.Count == 0)
            {
                _logger.LogWarning("No employees found.");
                return NotFound("No employees found.");
            }

            var employeesDTO = data.Select(emp => new EmployeeDTO
            {
                EmployeeId = emp.EmployeeId,
                Name = emp.Name,
                Email = emp.Email,
                Stack = emp.Stack,
                Role = emp.Role
            }).ToList();

            return Ok(employeesDTO);
        }


        /// <summary>
        /// This is the endpoint which gives you the details of specific employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        // GET: api/Employee/5


        [Authorize(Roles = "Admin,User")]
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            _logger.LogInformation("Fetching details for employee ID: {employeeId}", employeeId);

            var loggedInEmployeeId = int.Parse(User.FindFirst("EmployeeId")?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && loggedInEmployeeId != employeeId)
            {
                _logger.LogWarning("Unauthorized access by employee ID: {loggedInEmployeeId}");
                return Forbid();
            }

            var employee = await context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee ID: {employeeId} not found.", employeeId);
                return NotFound("Employee not found.");
            }

            var employeeDTO = new EmployeeDTO
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Email = employee.Email,
                Stack = employee.Stack,
                Role = employee.Role
            };

            return Ok(employeeDTO);
        }
        /// <summary>
        /// This will give you the access to create new instances in the employee database. Which takes The Employee object as input.
        /// But make sure you have the role as Admin
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> PostEmployee(EmployeePostDTO employeeDTO)
        {
            _logger.LogInformation("Adding new employee.");
            var salt = PasswordHasher.GenerateSalt();
            var hashedPassword = PasswordHasher.HashPassword(employeeDTO.Password, salt);

            var employee = new Employee
            {
                Name = employeeDTO.Name,
                Email = employeeDTO.Email,
                Stack = employeeDTO.Stack,
                Role = employeeDTO.Role ?? "User",
                Password = hashedPassword,
                Salt = salt
            };

            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            return Ok("Employee added successfully.");
        }

        /// <summary>
        /// Updates the details of an existing employee identified by Id.
        /// Requires Admin role.
        /// </summary>
        /// <param name="id">The Id of the employee to update.</param>
        /// <param name="updatedEmployee">The updated employee object containing the new details.</param>
        /// <returns>Returns the updated employee object if successful, or appropriate error messages otherwise.</returns>
        [Authorize(Roles = "Admin")]

        [Authorize(Roles = "Admin")]
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(int employeeId, EmployeePostDTO updatedEmployee)
        {
            _logger.LogInformation("Updating employee ID: {employeeId}", employeeId);

            var employee = await context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee ID: {employeeId} not found.", employeeId);
                return NotFound("Employee not found.");
            }

            var salt = PasswordHasher.GenerateSalt();
            var hashedPassword = PasswordHasher.HashPassword(updatedEmployee.Password, salt);

            employee.Name = updatedEmployee.Name;
            employee.Email = updatedEmployee.Email;
            employee.Stack = updatedEmployee.Stack;
            employee.Role = updatedEmployee.Role;
            employee.Password = hashedPassword;
            employee.Salt = salt;

            context.Employees.Update(employee);
            await context.SaveChangesAsync();

            return Ok("Employee updated successfully.");
        }
        /// <summary>
        /// Deletes the employee with the specified Id.
        /// Requires Admin role.
        /// </summary>
        /// <param name="id">The Id of the employee to delete.</param>
        /// <returns>Returns the deleted employee object if successful, or appropriate error messages otherwise.</returns>

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogInformation("Deleting employee ID: {id}", id);

            var employee = await context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == id);
            if (employee == null)
            {
                _logger.LogWarning("Employee ID: {id} not found.", id);
                return NotFound("Employee not found.");
            }

            context.Employees.Remove(employee);
            await context.SaveChangesAsync();

            return Ok("Employee deleted successfully.");
        }
    }
}
