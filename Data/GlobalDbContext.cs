using Comigle.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Comigle.Data
{
    public class GlobalDbContext : IdentityDbContext<User>
    {
        public GlobalDbContext(DbContextOptions<GlobalDbContext> opts) : base (opts)
        {
        }

        public override DbSet<User> Users { get; set; }
    }
}
