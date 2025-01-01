using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using WebApplication2.DataAcessLayer;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using WebApplication2.Middlewares;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(),"Log/log.txt",rollingInterval:RollingInterval.Day)
    .CreateLogger();

Log.Logger.Information("Logging is working fine.");

var builder = WebApplication.CreateBuilder(args);

//Log.Logger = new LoggerConfiguration().
//    MinimumLevel.Information()
//    .WriteTo.File("Log/log.txt",rollingInterval: RollingInterval.Minute)
//    .CreateLogger();
//builder.Services.AddSerilog();
// Add services to the container.
//builder.Logging.AddSerilog();
builder.Host.UseSerilog();

builder.Services.AddTransient<ExceptionMiddleware>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
     options =>
     {
         options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
         {
             Description = "Standard Authorization Header using the Bearer Scheme (\"bearer {token}\")",
             In = ParameterLocation.Header,
             Name = "Authorization",
             Type = SecuritySchemeType.ApiKey
         });

         options.OperationFilter<SecurityRequirementsOperationFilter>(); //Matt Frear
     }
);

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("MyConn")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
var app = builder.Build();


//static int sum(int a,int b)
//{
//    return a + b;
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();
app.CongigureExceptionMiddleware();

app.MapControllers();

app.Run();
