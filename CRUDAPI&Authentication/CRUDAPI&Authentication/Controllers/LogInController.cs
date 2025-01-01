using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebApplication2.DataAcessLayer;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LogInController> _logger;

        public LogInController(AppDbContext context, IConfiguration configuration, ILogger<LogInController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult GetToken(User user)
        {
            _logger.LogInformation("Received login request for user: {UserName}", user.Name);

            try
            {
                var data = _context.UserDetails.ToList();
                _logger.LogDebug("Fetched user details from database: {DataCount} records found.", data.Count);

                string token = "";

                if (data.Count > 0 && data[0].Name.Equals(user.Name) && data[0].Email.Equals(user.Email))
                {
                    _logger.LogInformation("User {UserName} authenticated successfully.", user.Name);
                    token = GenerateToken(user);
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for user: {UserName}", user.Name);
                    return Unauthorized("Invalid username or email.");
                }

                _logger.LogDebug("Generated token for user: {UserName}", user.Name);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the login request for user: {UserName}", user.Name);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        private string GenerateToken(User usr)
        {
            try
            {
                _logger.LogInformation("Generating token for user: {UserName}", usr.Name);

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddMinutes(1),
                    signingCredentials: credentials
                );

                _logger.LogDebug("Token generated successfully for user: {UserName}", usr.Name);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the token for user: {UserName}", usr.Name);
                throw;
            }
        }
    }
}

//private readonly AppDbContext _context;
//private readonly IConfiguration _configuration;
//public LoginController(AppDbContext context, IConfiguration configuration)
//{
//    _context = context;
//    _configuration = configuration;

//}
//[HttpPost]
//public IActionResult GetToken(User usr)
//{

//    var data = _context.UserDetails.ToList();
//    string token = "";
//    if (data[0].UserName.Equals(usr.UserName) && data[0].Password.Equals(usr.Password))
//    {
//        token = GenerateToken(usr);
//    }

//    return Ok(token);
//}

//private string GenerateToken(User usr)
//{

//    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

//    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
//    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null, expires: DateTime.Now.AddMinutes(1), signingCredentials: credentials);

//    return new JwtSecurityTokenHandler().WriteToken(token);
//}