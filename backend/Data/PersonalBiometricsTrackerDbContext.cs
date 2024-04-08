using Microsoft.EntityFrameworkCore;

namespace PersonalBiometricsTracker.Data
{
    public class PersonalBiometricsTrackerDbContext : DbContext
    {
        public PersonalBiometricsTrackerDbContext(DbContextOptions<PersonalBiometricsTrackerDbContext> options) : base(options)
        {

        }

    }
}