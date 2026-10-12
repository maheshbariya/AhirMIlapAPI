using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AhirMilapAPI.Controllers;

namespace AhirMilapAPI.Data
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
