using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace App.Infrastructure.Data
{
    public class AppDBContextFactory : IDesignTimeDbContextFactory<AppDBContext>
    {
        public AppDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=TestAPIDb;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=false");

            return new AppDBContext(optionsBuilder.Options);
        }
    }
}
