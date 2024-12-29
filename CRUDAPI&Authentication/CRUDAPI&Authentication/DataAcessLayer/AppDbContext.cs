using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.DataAcessLayer
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
: base(options) {
        
        }
        public DbSet<Employee> EmpDetails { get; set; }
        public DbSet<Student> StdDetails { get; set; }
        public DbSet<User> UserDetails { get; set; }

    }
}
