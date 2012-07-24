using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Transactions;

namespace BrewBuddy.Entities
{
    public class EntitiesContext
        : DbContext
    {
        public EntitiesContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Brew> Brews { get; set; }
        public DbSet<TemperatureAggregate> TemperatureAggregates { get; set; }
    }
}