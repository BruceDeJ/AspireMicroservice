using Microsoft.EntityFrameworkCore;
using TimeSheet.AuthAPI.Domain;

namespace AuthAPI
{
    public class IdentityContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }

        public string DbPath { get; }

        public IdentityContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "Identity.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserSession>().HasKey(x => x.UserSessionId);
            modelBuilder.Entity<UserSession>().HasOne(x => x.User)
                .WithMany(y => y.UserSessions)
                .HasForeignKey(z => z.UserId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
