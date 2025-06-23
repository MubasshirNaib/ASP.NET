using Backend.Core.Entities;
using Backend.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Application.Commands
{
    public record UpdateEmployeeCommand(Guid EmployeeId, EmployeeEntity employee)
       : IRequest<EmployeeEntity>;
    public class UpdateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        : IRequestHandler<UpdateEmployeeCommand, EmployeeEntity>
    {
        public async Task<EmployeeEntity> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            return await employeeRepository.UpdateEmployeeAsync(request.EmployeeId, request.employee);
        }
    }
}
