using Microsoft.EntityFrameworkCore;

namespace Labb4Anders
{
    public class AccountContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        private string URI = "https://localhost:8081";
        private string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private string database = "Accounts";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(URI, key, database);
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}