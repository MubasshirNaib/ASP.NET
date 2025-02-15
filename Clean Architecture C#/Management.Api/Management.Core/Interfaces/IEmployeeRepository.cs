using Management.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployees();
        Task<Employee> GetEmployeesByIdAsync(Guid id);
        Task<Employee> AddEmployeeAsync(Employee entity);
        Task<Employee> UpdateEmployeeAsync(Guid employeeId, Employee entity);
        Task<bool> DeleteEmployeeAsync(Guid employeeId);
    }
}
