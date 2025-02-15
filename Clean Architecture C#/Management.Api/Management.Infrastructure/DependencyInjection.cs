using Management.Core.Interfaces;
using Management.Infrastructure.Data;
using Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer("Server=localhost;Database=Management;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=false;");
            } );
            services.AddScoped<IEmployeeRepository,EmployeeRepository>();
            return services;
        }
    }
}
