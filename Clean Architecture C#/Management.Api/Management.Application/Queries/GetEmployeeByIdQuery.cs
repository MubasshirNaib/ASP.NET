using Management.Core.Entities;
using Management.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Application.Queries
{
    public record GetEmployeeByIdQuery(Guid employeeId): IRequest<Employee>;
    public class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
        : IRequestHandler<GetEmployeeByIdQuery, Employee>
    {
        public async Task<Employee> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            return await employeeRepository.GetEmployeesByIdAsync(request.employeeId);
        }
    }
}
