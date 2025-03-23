using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Domain.Users.Entities;

namespace WebAPI.Infrastructure.Users.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasOne(builder => builder.User)
                .WithMany()
                .HasForeignKey(builder => builder.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(builder => builder.RefreshTokenId);

            builder.Property(builder => builder.RefreshTokenId)
                .HasMaxLength(256)
                .IsRequired();


            builder.Property(builder => builder.Token)
                .HasMaxLength(256)
                .IsRequired();

        }
    }
}
