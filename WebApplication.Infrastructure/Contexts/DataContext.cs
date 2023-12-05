using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication.Infrastructure.Entities;

namespace WebApplication.Infrastructure.Contexts
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString"));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
    }
}
