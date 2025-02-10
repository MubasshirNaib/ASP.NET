using Management.Core.Entities;
using Management.Core.Interfaces;
using Management.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Infrastructure.Repositories
{
    public class EmployeeRepository(AppDbContext dbContext):IEmployeeRepository
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
            entity.EmployeeId=Guid.NewGuid(); 
            dbContext.Employees.Add(entity);
            await dbContext.SaveChangesAsync();
            return entity; 
        }
        public async Task<Employee> UpdateEmployeeAsync(Guid employeeId,Employee entity)
        {
            var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
            if (employee is not null)
            {
                employee.EmployeeName=entity.EmployeeName; ;
                employee.Stack=entity.Stack;
                employee.Mobile=entity.Mobile;
                employee.Email=entity.Email;
                employee.Password=entity.Password;
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
    }
}
