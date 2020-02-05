using Microsoft.EntityFrameworkCore;

namespace Labb4Anders
{
    public class AccountContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        private string URI = "https://labb4db.documents.azure.com:443/";
        private string key = "mkh6phIc5Rv9RpAr8FjqpLGvj7qD9SN8cshVav7rRh5w7KwKP5l9e1eoWtbLMdBaMwXTLdhuehu00nnAgxJuRA==";
        private string database = "Accounts";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(URI, key, database);
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}