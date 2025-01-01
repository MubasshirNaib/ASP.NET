using EmpTaskAPI.DataAccessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Serilog;
using Serilog.Events;
using System.Reflection;
//using System.Text.Json.Serializatio;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
//new CompactJsonFormatter()
Log.Logger.Information("Logging is working fine.");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
//builder.Services.AddTransient<GlobalExceptionMiddleware>();

// Add services to the container.

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
         options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
         options.OperationFilter<SecurityRequirementsOperationFilter>(); //Matt Frear
         options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
     }

     
);



//builder.Services.AddResponseCaching(x=> x.MaximumBodySize=1024 );

builder.Services.AddSwaggerGen();
// Check if you're using Newtonsoft.Json elsewhere in your code
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allows any origin
              .AllowAnyMethod()  // Allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader(); // Allows any header
    });
});

builder.Services.AddDbContext<AppDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Dbconn")));

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




//var app = builder.Build();
//app.UseMiddleware<GlobalExceptionMiddleware>();



builder.Services.AddDistributedMemoryCache(); // For in-memory session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Session timeout
    options.Cookie.HttpOnly = true; // Prevent client-side script access
    options.Cookie.IsEssential = true; // Ensure session cookies work without consent
});

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpLogging();
app.UseSession();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


//app.UseResponseCaching();
//app.Use(async (context, next) =>
//{
//    context.Response.GetTypedHeaders().CacheControl =
//    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
//    {
//        Public = true,
//        MaxAge = TimeSpan.FromSeconds(10)

//    };
//    await next();
//    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary]= new String[] {"Accept-Encoding"};


//});
app.Run();
