using Comigle.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Comigle.Data
{
    public class ComigleDbContext : IdentityDbContext<User>
    {
        public ComigleDbContext(DbContextOptions<ComigleDbContext> opts) : base (opts)
        {
        }

        public override DbSet<User> Users { get; set; }
    }
}
