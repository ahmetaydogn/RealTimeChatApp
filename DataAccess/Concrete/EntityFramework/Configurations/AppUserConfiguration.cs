using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.EntityFramework.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.Username).IsUnique();

            builder.Property(x => x.UserType).HasConversion<int>();
        }
    }
}
