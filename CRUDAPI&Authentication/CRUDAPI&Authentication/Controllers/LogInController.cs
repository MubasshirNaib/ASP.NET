using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebApplication2.Controllers;
using WebApplication2.DataAcessLayer;
using WebApplication2.Migrations;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public LogInController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult GetToken(User user)
        {
            var data= _context.UserDetails.ToList();
            string token = "";
            if (data[0].Name.Equals(user.Name)&& data[0].Email.Equals(user.Email) )
            {
                token = GenerateToken(user);

            }
            return Ok(token);
        }
        private string GenerateToken(User usr)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null, expires: DateTime.Now.AddMinutes(1), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
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