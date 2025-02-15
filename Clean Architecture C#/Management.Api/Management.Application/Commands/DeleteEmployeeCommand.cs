using Management.Core.Interfaces;
using MediatR;

namespace Management.Application.Commands
{
    public record DeleteEmployeeCommand(Guid EmployeeId): IRequest<bool>;
    public class DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository)
         : IRequestHandler<DeleteEmployeeCommand, bool>
    {
        public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            return await employeeRepository.DeleteEmployeeAsync(request.EmployeeId);
        }
    }
}
