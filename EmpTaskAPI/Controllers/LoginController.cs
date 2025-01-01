using EmpTaskAPI.DataAccessLayer;
using EmpTaskAPI.HashPassword;
using EmpTaskAPI.Models;
using EmpTaskAPI.DTOModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Serilog;

namespace EmpTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProjectController> _logger;

        public LoginController(AppDBContext context, IConfiguration configuration, ILogger<ProjectController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        /// <summary>
        /// Authenticates a user and generates an access token and a refresh token.
        /// </summary>
        /// <param name="emp">The login details including email and password.</param>
        /// <returns>
        /// An object containing the access token, refresh token, employee ID, and role if authentication is successful;
        /// otherwise, a BadRequest or Unauthorized response.
        /// </returns>

        [HttpPost]
        public async Task<IActionResult> GetToken(UserLogin emp)
        {
            Log.Information("Login attempt for email: {Email}", emp.Email);

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == emp.Email);
            if (employee == null)
            {
                Log.Warning("Invalid email attempt: {Email}", emp.Email);
                return BadRequest("Invalid Email");
            }

            var isValidPassword = PasswordHasher.HashPassword(emp.Password, employee.Salt) == employee.Password;
            if (!isValidPassword)
            {
                Log.Warning("Invalid password attempt for email: {Email}", emp.Email);
                return Unauthorized("Invalid email or password");
            }

            string token = GenerateToken(employee);
            string refreshToken = GenerateRefreshToken();

            employee.RefreshToken = refreshToken;
            employee.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            Log.Information("Login successful for email: {Email}", emp.Email);

            return Ok(new
            {
                Token = token,
                RefreshToken = refreshToken,
                EmployeeId = employee.EmployeeId,
                Role = employee.Role
            });
        }

        private string GenerateToken(Employee emp)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, emp.Name),
                new Claim(ClaimTypes.Role, emp.Role),
                new Claim("EmployeeId", emp.EmployeeId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
    }
}
