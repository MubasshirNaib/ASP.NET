using Backend.Core.DTO;
using Backend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeeEntity>> GetEmployees();
        Task<EmployeeEntity> GetEmployeesByIdAsync(Guid id);
        Task<EmployeeEntity> AddEmployeeAsync(EmployeeEntity entity);
        Task<EmployeeEntity> UpdateEmployeeAsync(Guid employeeId, EmployeeEntity entity);
        Task<bool> DeleteEmployeeAsync(Guid employeeId);
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest request);
    }
}
