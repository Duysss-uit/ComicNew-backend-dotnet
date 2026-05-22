using ComicNew.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComicNew.Infrastructure.Persistence.Configurations;

public class ReadingHistoryConfiguration : IEntityTypeConfiguration<ReadingHistory>
{
    public void Configure(EntityTypeBuilder<ReadingHistory> builder)
    {
        builder.HasKey(rh => rh.Id);

        builder.HasIndex(rh => new { rh.UserId, rh.ChapterNumber })
            .IsUnique();

        builder.HasOne(rh => rh.User)
            .WithMany(u => u.ReadingHistory)
            .HasForeignKey(rh => rh.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rh => rh.Story)
            .WithMany()
            .HasForeignKey(rh => rh.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(rh => rh.ChapterNumber)
            .IsRequired();

        builder.Property(rh => rh.ReadAt)
            .HasDefaultValueSql("NOW()");
    }
}