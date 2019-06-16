using Microsoft.EntityFrameworkCore;

namespace Kaffee.Models
{
    public class CoffeeContext : DbContext
    {
        public CoffeeContext(DbContextOptions<CoffeeContext> options)
            : base(options)
        {
        }

        public DbSet<Coffee> Coffees { get; set; }
    }
}