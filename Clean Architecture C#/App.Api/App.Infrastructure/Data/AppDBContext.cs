
using App.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        // Add DbSet properties for your entities here
        // Example:
        // public DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<EmployeeEntity> Employees { get; set; }
    }
}
