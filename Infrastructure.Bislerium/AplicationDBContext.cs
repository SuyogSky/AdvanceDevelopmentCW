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
    public class AplicationDBContext: IdentityDbContext<AppUser>
    {
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<BlogNotification> BlogNotifications { get; set; }

        public DbSet<BlogHistory> BlogHistory { get; set; }
        public DbSet<CommentHistory> CommentHistory { get; set; }

        public DbSet<BlogReaction> BlogReactions { get; set; }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
        public DbSet<MobileTokens> MobileTokens { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=WIN-NKF4P1JLCQQ;Database=Bislerium;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");
        }
        public DbSet<Student> Students { get; set; }
    }
}
