using ComicNew.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComicNew.Infrastructure.Persistence.Configurations;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.ChapterNumber)
            .IsRequired();

        builder.Property(c => c.ImageUrls)
            .HasColumnType("text[]");

        builder.Property(c => c.Views)
            .HasDefaultValue(0);

        builder.Property(c => c.PublishedAt)
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(c => c.StoryId);

        builder.HasOne(c => c.Story)
            .WithMany(s => s.Chapters)
            .HasForeignKey(c => c.StoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}