using Management.Core.Entities;
using Management.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Application.Commands
{
    public record UpdateEmployeeCommand(Guid EmployeeId,Employee employee)
        :IRequest<Employee>;
    public class UpdateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        : IRequestHandler<UpdateEmployeeCommand, Employee>
    {
        public async Task<Employee> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            return await employeeRepository.UpdateEmployeeAsync(request.EmployeeId, request.employee);
        }
    }

    
}
