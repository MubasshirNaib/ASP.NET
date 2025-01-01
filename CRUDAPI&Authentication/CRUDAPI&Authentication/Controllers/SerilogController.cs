using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication2.Exceptions;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerilogController : ControllerBase
    {
        private readonly ILogger<SerilogController> _logger;

        public SerilogController(ILogger<SerilogController> logger)
        {
            _logger = logger;
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (id == 1)
            {
                int a = 5;
                int c = a / 0;
            }
            else if (id == 2)
            {
                throw new NotFoundException("record does not found");
            }
            else if (id == 3)
            {
                int a = 3;
            }
            else
            {
                throw new BadRequestException("Bad request");
            }

            return Ok();
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex.Message);
            //    return StatusCode(500);
            //}
        }
    }
}
