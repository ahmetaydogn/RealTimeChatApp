using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework.Contexts
{
    public class RealTimeChatAppContext : DbContext
    {
        public RealTimeChatAppContext(DbContextOptions<RealTimeChatAppContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-18AEIRM\SQLEXPRESS01\TrustedConnection=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                b.HasIndex(x => x.Username).IsUnique();

                b.Property(x => x.UserType).HasConversion<short>();
            });
        }

        DbSet<AppUser> AppUsers { get; set; }

    }
}
