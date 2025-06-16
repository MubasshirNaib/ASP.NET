using Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace Backend.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<EmployeeEntity> Employees { get; set; }

    }
}
