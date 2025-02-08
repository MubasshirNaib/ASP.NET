using App.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<EmployeeEntity> Employees { get; set; }
    }
}
