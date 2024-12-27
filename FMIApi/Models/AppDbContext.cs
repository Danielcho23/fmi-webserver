using Microsoft.EntityFrameworkCore;

namespace FMIApi.Models
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Car> Cars { get; set; }

        public DbSet<Garage> Garages { get; set; }

        public DbSet<Maintenance> Maintenances { get; set; }
    }
}
