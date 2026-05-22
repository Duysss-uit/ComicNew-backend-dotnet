using ComicNew.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComicNew.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(2048);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasMany(u => u.Stories)
            .WithOne(s => s.Author)
            .HasForeignKey(s => s.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ReadingHistory)
            .WithOne(rh => rh.User)
            .HasForeignKey(rh => rh.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}