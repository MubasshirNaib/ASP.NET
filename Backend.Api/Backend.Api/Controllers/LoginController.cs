using Backend.Application.Commands;
using Backend.Core.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(ISender sender) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> Authentication([FromBody] AuthenticationRequest employee)
        {
            var result = await sender.Send(new LoginCommand(employee));
            return Ok(result);
        }
    }
}
