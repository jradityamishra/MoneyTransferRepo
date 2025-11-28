using AccountMicroservices.Data.Model;
using Microsoft.EntityFrameworkCore;
//using UserMicroservices.Model.Entity;
namespace UserMicroservices.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }


        public DbSet<Account> Accounts { get; set; }
    }
}
