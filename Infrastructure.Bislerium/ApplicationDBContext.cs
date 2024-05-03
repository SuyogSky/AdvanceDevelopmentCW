using Domain.Bislerium;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=WIN-NKF4P1JLCQQ;Database=Bislerium;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");
        }
        public DbSet<Blog> Blog { get; set; }
    }
}
