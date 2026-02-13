using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Domain.Models;

namespace Restaurant.Infrastructure.EF
{
    public sealed class DemoContext : DbContext
    {
        public DemoContext(DbContextOptions<DemoContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
