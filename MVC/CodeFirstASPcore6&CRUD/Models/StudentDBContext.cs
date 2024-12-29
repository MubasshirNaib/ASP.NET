using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace CodeFirstASPcore6.Models
{
    public class StudentDBContext : DbContext
    {
        public StudentDBContext(DbContextOptions options) : base(options)
        {
                
        }

        public DbSet<Student> Students { get; set; }
    }
}
