using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.EntityFramework.Configurations
{
    public class RoomMemberConfiguration : IEntityTypeConfiguration<RoomMember>
    {
        public void Configure(EntityTypeBuilder<RoomMember> builder)
        {
            builder.HasKey(x => new { x.RoomId, x.UserId });

            builder.Property(x => x.Role).HasConversion<int>();

            builder.HasOne(x => x.Room)
                .WithMany(r => r.Members)
                .HasForeignKey(x => x.RoomId);
                //.OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(u => u.RoomMembers)
                .HasForeignKey(x => x.UserId);
        }
    }
}
