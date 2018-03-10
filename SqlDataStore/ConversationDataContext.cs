using SqlDataStore.Models;
using System.Data.Entity;

namespace SqlDataStore
{
    public class ConversationDataContext : DbContext
    {
        public ConversationDataContext()
            : base("ConversationDataContextConnectionString")
        {
        }
        public DbSet<Activity> Activities { get; set; }
    }
}