using Management.Core.DTO;
using Management.Core.Entities;
using Management.Core.Interfaces;
using Management.Infrastructure.Data;
using Management.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Management.Infrastructure.Repositories
{
    public class EmployeeRepository(AppDbContext dbContext,IConfiguration _configuration):IEmployeeRepository
    {
       

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await dbContext.Employees.ToListAsync();
        }
        public async Task<Employee> GetEmployeesByIdAsync(Guid id)
        {
            return await dbContext.Employees.FirstOrDefaultAsync(x=>x.EmployeeId == id);
        }
        public async Task<Employee> AddEmployeeAsync(Employee entity)
        {
            var salt = PasswordHasher.GenerateSalt();
            var hashedPassword = PasswordHasher.HashPassword(entity.Password, salt);
            entity.EmployeeId=Guid.NewGuid();
            entity.Password = hashedPassword;
            entity.Salt = salt; 
            dbContext.Employees.Add(entity);
            await dbContext.SaveChangesAsync();
            return entity; 
        }
        public async Task<Employee> UpdateEmployeeAsync(Guid employeeId,Employee entity)
        {
            var salt = PasswordHasher.GenerateSalt();
            var hashedPassword = PasswordHasher.HashPassword(entity.Password, salt);
            var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
            if (employee is not null)
            {
                employee.EmployeeName=entity.EmployeeName; ;
                employee.Stack=entity.Stack;
                employee.Mobile=entity.Mobile;
                employee.Email=entity.Email;
                employee.Password=hashedPassword;
                employee.Salt = salt;
                employee.JoiningDate=entity.JoiningDate;
                employee.Image=entity.Image;

                await dbContext.SaveChangesAsync();
                return employee;
            }
            
            return entity;
        }
        public async Task<bool> DeleteEmployeeAsync(Guid employeeId)
        {
            var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
            if (employee is not null)
            {
               dbContext.Employees.Remove(employee); ;

                return await dbContext.SaveChangesAsync() >0;
                
            }

            return false;
        }
        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request)
        {

            var user = await dbContext.Employees.FirstOrDefaultAsync(x=>x.Email == request.Email);
            if (user == null) {
                throw new ApplicationException($"user is not found with this Email : {request.Email}");
            }
            var password = PasswordHasher.HashPassword(request.Password,user.Salt);
            var succeed = await dbContext.Employees.FirstOrDefaultAsync(x => x.Password == password);
            if (succeed == null)
            {
                throw new ApplicationException($"Password isn't correct");
               
            }
            var JwtSecurity = await GenerateToken(user);
            var authenticationResponse = new AuthenticationResponse();
            authenticationResponse.Role = "Admin";
            authenticationResponse.JwToken = new JwtSecurityTokenHandler().WriteToken(JwtSecurity);
            return authenticationResponse;
        }

        private async Task <JwtSecurityToken> GenerateToken(Employee employee)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.EmployeeName),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("EmployeeId", employee.EmployeeId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

            return token;
        }
    }
}
