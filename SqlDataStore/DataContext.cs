using SqlDataStore.Models;
using System.Data.Entity;
using LuisBot.Models;

namespace SqlDataStore
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("ConnectionString")
        {
        }
        public DbSet<Activity> Activities { get; set; }

        public DbSet<UserDetail> UserDetails { get; set; }

        public DbSet<CreditCard> CreditCards { get; set; }
    }
}