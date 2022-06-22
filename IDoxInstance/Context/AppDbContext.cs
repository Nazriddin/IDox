using IDoxInstance.Entities;
using Microsoft.EntityFrameworkCore;

namespace IDoxInstance.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public AppDbContext()
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
    }
}
