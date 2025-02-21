using Management.Application.Commands;
using Management.Application.Queries;
using Management.Core.DTO;
using Management.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Management.Application.Commands;
using Management.Application.Queries;
using Management.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(ISender sender):ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> Authentication([FromBody] AuthenticationRequest employee)
        {
            var result = await sender.Send(new LoginCommand(employee));
            return Ok(result);
        }
    }
}
