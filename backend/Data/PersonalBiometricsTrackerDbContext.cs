using Microsoft.EntityFrameworkCore;
using PersonalBiometricsTracker.Entities;

namespace PersonalBiometricsTracker.Data
{
    public class PersonalBiometricsTrackerDbContext : DbContext
    {
        public PersonalBiometricsTrackerDbContext(DbContextOptions<PersonalBiometricsTrackerDbContext> options) : base(options)
        {

        }

        // Db Sets
        public DbSet<User> Users { get; set; }
        public DbSet<Weight> Weights { get; set; }

    }
}