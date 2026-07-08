using ComicNew.Domain.Entities;
using ComicNew.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComicNew.Infrastructure.Persistence.Configurations;

public class StoryConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.Description)
            .HasMaxLength(4000);

        builder.Property(s => s.CoverUrl)
            .HasMaxLength(2048);


        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(StoryType.Comic);

        builder.Property(s => s.Views)
            .HasDefaultValue(0);

        builder.Property(s => s.Rating)
            .HasDefaultValue(0.0);

        builder.HasIndex(s => s.Title);
        builder.HasIndex(s => s.AuthorId);

        builder.HasOne(s => s.Author)
            .WithMany(u => u.Stories)
            .HasForeignKey(s => s.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Chapters)
            .WithOne(c => c.Story)
            .HasForeignKey(c => c.StoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}