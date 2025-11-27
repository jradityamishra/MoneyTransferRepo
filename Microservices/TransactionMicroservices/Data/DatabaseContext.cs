using Microsoft.EntityFrameworkCore;
using TransactionMicroservices.Model.Entity;

namespace UserMicroservices.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }


        public DbSet<TransactionSchema> TransactionDB { get; set; }
    }
}
