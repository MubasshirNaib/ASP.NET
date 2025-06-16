using Azure.Identity;
using Backend.Core.DTO;
using Backend.Core.Entities;
using Backend.Core.Exceptions;
using Backend.Core.Interfaces;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(AppDbContext dbContext, IConfiguration configuration, ILogger<EmployeeRepository> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeEntity>> GetEmployees()
        {
            _logger.LogInformation("Retrieving all employees.");
            var employees = await _dbContext.Employees.ToListAsync();
            _logger.LogInformation("Retrieved {EmployeeCount} employees.", employees.Count);
            return employees;
        }

        public async Task<EmployeeEntity> GetEmployeesByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving employee with ID {EmployeeId}.", id);
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", id);
                throw new NotFoundException("Employee", id);
            }
            _logger.LogInformation("Successfully retrieved employee with ID {EmployeeId}.", id);
            return employee;
        }

        public async Task<EmployeeEntity> AddEmployeeAsync(EmployeeEntity entity)
        {
            _logger.LogInformation("Adding new employee with email {Email}.", entity.Email);
            bool emailExists = await _dbContext.Employees.AnyAsync(e => e.Email == entity.Email);

            if (emailExists)
            {
                _logger.LogError("Email {Email} is already in use.", entity.Email);
                throw new EmailAlreadyExistsException(entity.Email);
            }

            entity.Id = Guid.NewGuid();
            _dbContext.Employees.Add(entity);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully added employee with ID {EmployeeId} and email {Email}.", entity.Id, entity.Email);
            return entity;
        }

        public async Task<EmployeeEntity> UpdateEmployeeAsync(Guid employeeId, EmployeeEntity entity)
        {
            _logger.LogInformation("Updating employee with ID {EmployeeId}.", employeeId);
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == employeeId);
            if (employee is not null)
            {
                // Check for email uniqueness (excluding the current employee)
                bool emailExists = await _dbContext.Employees
                    .AnyAsync(e => e.Email == entity.Email && e.Id != employeeId);
                if (emailExists)
                {
                    _logger.LogError("Email {Email} is already in use by another employee.", entity.Email);
                    throw new EmailAlreadyExistsException(entity.Email);
                }

                employee.Name = entity.Name;
                employee.Email = entity.Email;
                employee.Phone = entity.Phone;
                employee.Password = entity.Password;

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully updated employee with ID {EmployeeId}.", employeeId);
                return employee;
            }

            _logger.LogWarning("Employee with ID {EmployeeId} not found for update.", employeeId);
            throw new NotFoundException("Employee", employeeId);
        }

        public async Task<bool> DeleteEmployeeAsync(Guid employeeId)
        {
            _logger.LogInformation("Deleting employee with ID {EmployeeId}.", employeeId);
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == employeeId);
            if (employee is not null)
            {
                _dbContext.Employees.Remove(employee);
                var rowsAffected = await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted employee with ID {EmployeeId}.", employeeId);
                return rowsAffected > 0;
            }

            _logger.LogWarning("Employee with ID {EmployeeId} not found for deletion.", employeeId);
            throw new NotFoundException("Employee", employeeId);
        }

        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
        {
            _logger.LogInformation("Authenticating user with email {Email}.", request.Email);
            var user = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
            {
                _logger.LogError("Authentication failed: User with email {Email} not found.", request.Email);
                throw new Core.Exceptions.AuthenticationFailedException($"User is not found with this Email: {request.Email}");
            }

            var succeed = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Password == request.Password && x.Email == request.Email);
            if (succeed == null)
            {
                _logger.LogError("Authentication failed: Incorrect password for email {Email}.", request.Email);
                throw new Core.Exceptions.AuthenticationFailedException("Password isn't correct");
            }

            var jwtSecurity = await GenerateToken(user);
            var authenticationResponse = new AuthenticationResponse
            {
                Role = "Admin",
                JwToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurity)
            };
            _logger.LogInformation("Successfully authenticated user with email {Email}.", request.Email);
            return authenticationResponse;
        }

        private async Task<JwtSecurityToken> GenerateToken(EmployeeEntity employee)
        {
            _logger.LogInformation("Generating JWT token for employee with ID {EmployeeId}.", employee.Id);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.Name),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("Id", employee.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

            _logger.LogInformation("Successfully generated JWT token for employee with ID {EmployeeId}.", employee.Id);
            return token;
        }
    }
}