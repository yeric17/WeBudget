using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Domain.Users;

namespace WebAPI.Infrastructure.Users
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.Property(u => u.NickName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasDefaultValue(DateTimeOffset.UtcNow)
                .IsRequired();
        }
    }
}
