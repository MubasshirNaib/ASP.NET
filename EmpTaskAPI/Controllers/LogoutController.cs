using EmpTaskAPI.DataAccessLayer;
using EmpTaskAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EmpTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ILogger<ProjectController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutController"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        public LogoutController(AppDBContext context, ILogger<ProjectController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Logs out the user by invalidating the refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to invalidate.</param>
        /// <returns>An IActionResult indicating the result of the logout operation.</returns>
        /// <response code="200">Successfully logged out and refresh token invalidated.</response>
        /// <response code="400">Invalid refresh token provided.</response>
        /// <response code="500">An unexpected error occurred.</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            Log.Information("Logout attempt with refresh token: {RefreshToken}", refreshToken);

            // Validate the refresh token
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.RefreshToken == refreshToken);
            if (employee == null)
            {
                Log.Warning("Invalid logout attempt with refresh token: {RefreshToken}", refreshToken);
                return BadRequest("Invalid refresh token");
            }

            // Invalidate the refresh token
            employee.RefreshToken = null;
            employee.RefreshTokenExpiry = DateTime.MinValue; // Optionally reset the expiry date

            // Save changes to the database
            await _context.SaveChangesAsync();

            Log.Information("Logout successful for employee ID: {EmployeeId}", employee.EmployeeId);
            return Ok("Logged out successfully");
        }
    }
}
