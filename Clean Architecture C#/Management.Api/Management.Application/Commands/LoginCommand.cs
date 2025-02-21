using Management.Core.Entities;
using MediatR;
using Management.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Management.Core.DTO;

namespace Management.Application.Commands
{
    public record LoginCommand(AuthenticationRequest response) : IRequest<AuthenticationResponse>;
    public class LoginCommandHandler(IEmployeeRepository employeeRepository)
        : IRequestHandler<LoginCommand, AuthenticationResponse>
    {
        public async Task<AuthenticationResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return  await employeeRepository.Authenticate(request.response);
        }
    }
}
