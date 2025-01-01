using System.Security.Claims;
using EmpTaskAPI.DataAccessLayer;
using EmpTaskAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EmpTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ILogger<ProjectController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectController"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        public ProjectController(AppDBContext context, ILogger<ProjectController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of all projects.
        /// </summary>
        /// <returns>A list of projects if found, otherwise a NotFound result.</returns>
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> GetProjects()
        {
            try
            {
                _logger.LogInformation("GetProjects endpoint called.");
                var data = await _context.Projects.ToListAsync();

                if (data == null)
                {
                    _logger.LogWarning("No projects found.");
                    return NotFound("Data not found.");
                }

                _logger.LogInformation("Successfully retrieved all projects.");
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetProjects.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the project details for a specific employee.
        /// Requires Admin or User role.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>The project details if found and authorized, otherwise Unauthorized or NotFound result.</returns>
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{employeeId}")]
        public async Task<ActionResult> GetProjectById(int employeeId)
        {
            try
            {
                _logger.LogInformation("GetProjectById endpoint called by user: {User} for employeeId: {EmployeeId}.", User.Identity?.Name, employeeId);

                var loggedInEmployeeId = int.Parse(User.FindFirst("EmployeeId")?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                var assignedTask = await _context.AssignedTasks.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
                var task = await _context.Tasks.FirstOrDefaultAsync(x => x.TaskId == assignedTask.TaskId);
                var project = await _context.Projects.FirstOrDefaultAsync(x => x.ProjectId == task.ProjectId);

                if (project == null)
                {
                    _logger.LogWarning("Project not found or unauthorized access for employeeId: {EmployeeId}.", employeeId);
                    return Unauthorized();
                }

                if (userRole != "Admin" && employeeId != loggedInEmployeeId)
                {
                    _logger.LogWarning("Unauthorized access by user: {User} for employeeId: {EmployeeId}.", User.Identity?.Name, employeeId);
                    return Unauthorized();
                }

                _logger.LogInformation("Successfully retrieved project for employeeId: {EmployeeId}.", employeeId);
                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetProjectById.");
                throw;
            }
        }

        /// <summary>
        /// Creates a new project.
        /// Requires Admin role.
        /// </summary>
        /// <param name="project">The project object containing project details.</param>
        /// <returns>Success message if the project is created successfully.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> PostProject(Project project)
        {
            try
            {
                _logger.LogInformation("PostProject endpoint called by user: {User}.", User.Identity?.Name);

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created new project with title: {Title}.", project.Title);
                return Ok("Done");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in PostProject.");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing project.
        /// Requires Admin role.
        /// </summary>
        /// <param name="id">The ID of the project to update.</param>
        /// <param name="updatedProject">The updated project object containing new details.</param>
        /// <returns>The updated project object if successful, otherwise an appropriate error result.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, Project updatedProject)
        {
            try
            {
                _logger.LogInformation("UpdateUser endpoint called by user: {User} for projectId: {ProjectId}.", User.Identity?.Name, id);

                var project = await _context.Projects.FirstOrDefaultAsync(x => x.ProjectId == id);
                if (project == null)
                {
                    _logger.LogWarning("Project with projectId: {ProjectId} not found.", id);
                    return NotFound("Project Data not found.");
                }

                project.StartDate = updatedProject.StartDate;
                project.EndDate = updatedProject.EndDate;
                project.Title = updatedProject.Title;
                project.Description = updatedProject.Description;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated project with projectId: {ProjectId}.", id);
                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in UpdateUser.");
                throw;
            }
        }

        /// <summary>
        /// Deletes a specific project.
        /// Requires Admin role.
        /// </summary>
        /// <param name="id">The ID of the project to delete.</param>
        /// <returns>Success message if the project is deleted successfully, otherwise an appropriate error result.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                _logger.LogInformation("DeleteProject endpoint called by user: {User} for projectId: {ProjectId}.", User.Identity?.Name, id);

                var data = await _context.Projects.FirstOrDefaultAsync(x => x.ProjectId == id);
                if (data == null)
                {
                    _logger.LogWarning("Project with projectId: {ProjectId} not found.", id);
                    return NotFound();
                }

                _context.Projects.Remove(data);
                var relatedTasks = _context.Tasks.Where(t => t.ProjectId == id);
                _context.Tasks.RemoveRange(relatedTasks);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted project with projectId: {ProjectId}.", id);
                return Ok("Successfully Deleted!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteProject.");
                throw;
            }
        }

        /// <summary>
        /// Deletes all projects and related tasks.
        /// Requires Admin role.
        /// </summary>
        /// <returns>Success message if all projects are deleted successfully, otherwise an appropriate error result.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllProjects()
        {
            _logger.LogInformation("DeleteAllProjects endpoint called by user: {User}.", User.Identity?.Name);

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var projectIds = await _context.Projects.Select(p => p.ProjectId).ToListAsync();
                var taskIds = await _context.Tasks.Where(t => projectIds.Contains(t.ProjectId)).Select(t => t.TaskId).ToListAsync();

                _logger.LogDebug("Deleting assigned tasks.");
                _context.AssignedTasks.RemoveRange(_context.AssignedTasks.Where(at => taskIds.Contains(at.TaskId)));

                _logger.LogDebug("Deleting tasks.");
                _context.Tasks.RemoveRange(_context.Tasks.Where(t => projectIds.Contains(t.ProjectId)));

                _logger.LogDebug("Deleting projects.");
                _context.Projects.RemoveRange(_context.Projects.Where(p => projectIds.Contains(p.ProjectId)));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully deleted all projects and related tasks.");
                return Ok("Deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteAllProjects.");
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
