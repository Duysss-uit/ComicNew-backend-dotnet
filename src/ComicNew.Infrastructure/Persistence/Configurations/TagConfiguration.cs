using ComicNew.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComicNew.Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.Slug).IsUnique();

        // Many-to-many relationship with Story
        // EF Core 5+ supports many-to-many without explicit join entity
        builder.HasMany(t => t.Stories)
            .WithMany(s => s.Tags)
            .UsingEntity(j => j.ToTable("StoryTags"));

        builder.HasData(
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Hành Động", Slug = "hanh-dong", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Phiêu Lưu", Slug = "phieu-luu", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Hài Hước", Slug = "hai-huoc", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Chính Kịch", Slug = "chinh-kich", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Giả Tưởng", Slug = "gia-tuong", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Name = "Lãng Mạn", Slug = "lang-man", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), Name = "Isekai (Chuyển Sinh)", Slug = "isekai-chuyen-sinh", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), Name = "Kinh Dị", Slug = "kinh-di", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), Name = "Bí Ẩn", Slug = "bi-an", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Name = "Đời Thường", Slug = "doi-thuong", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Name = "Học Đường", Slug = "hoc-duong", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Name = "Siêu Nhiên", Slug = "sieu-nhien", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Name = "Tâm Lý", Slug = "tam-ly", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Name = "Khoa Học Viễn Tưởng", Slug = "khoa-hoc-vien-tuong", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), Name = "Bi Kịch", Slug = "bi-kich", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000016"), Name = "Võ Thuật", Slug = "vo-thuat", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000017"), Name = "Lịch Sử", Slug = "lich-su", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000018"), Name = "Thể Thao", Slug = "the-thao", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000019"), Name = "Manhwa", Slug = "manhwa", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000020"), Name = "Manhua", Slug = "manhua", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000021"), Name = "Webtoon", Slug = "webtoon", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000022"), Name = "Josei", Slug = "josei", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000023"), Name = "Seinen", Slug = "seinen", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000024"), Name = "Shounen", Slug = "shounen", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Tag { Id = Guid.Parse("00000000-0000-0000-0000-000000000025"), Name = "Shoujo", Slug = "shoujo", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
