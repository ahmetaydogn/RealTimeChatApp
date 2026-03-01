using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.EntityFramework.Configurations
{
    public class RoomMemberConfiguration : IEntityTypeConfiguration<RoomMember>
    {
        public void Configure(EntityTypeBuilder<RoomMember> builder)
        {
            builder.HasKey(x => new { x.RoomId, x.UserId }); // composite key

            builder.Property(x => x.Role)
                .HasConversion<int>();

            builder.HasOne(x => x.User)
                .WithMany(x => x.RoomMembers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
