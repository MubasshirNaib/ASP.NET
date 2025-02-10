using Management.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options ) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }
        
    }
}
