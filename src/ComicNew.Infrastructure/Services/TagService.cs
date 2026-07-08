using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ComicNew.Application.DTOs.Tags;
using ComicNew.Application.Interfaces;
using ComicNew.Domain.Entities;
using ComicNew.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ComicNew.Infrastructure.Services;

public class TagService : ITagService
{
    private readonly AppDbContext _db;

    public TagService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TagDto>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        var tags = await _db.Tags.OrderBy(t => t.Name).ToListAsync(cancellationToken);
        return tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
            Slug = t.Slug
        }).ToList();
    }

    public async Task<TagDto?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tag == null) return null;

        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Slug = tag.Slug
        };
    }

    public async Task<TagDto> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken = default)
    {
        var slug = string.IsNullOrWhiteSpace(request.Slug) ? GenerateSlug(request.Name) : request.Slug;

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = slug,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Tags.Add(tag);
        await _db.SaveChangesAsync(cancellationToken);

        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Slug = tag.Slug
        };
    }

    public async Task<TagDto?> UpdateTagAsync(Guid id, UpdateTagRequest request, CancellationToken cancellationToken = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tag == null) return null;

        tag.Name = request.Name;
        tag.Slug = string.IsNullOrWhiteSpace(request.Slug) ? GenerateSlug(request.Name) : request.Slug;
        tag.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Slug = tag.Slug
        };
    }

    public async Task<bool> DeleteTagAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tag == null) return false;

        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }

    public static string GenerateSlug(string phrase)
    {
        string str = RemoveDiacritics(phrase).ToLower();
        // invalid chars           
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        // convert multiple spaces into one space   
        str = Regex.Replace(str, @"\s+", " ").Trim();
        // cut and trim 
        str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
        str = Regex.Replace(str, @"\s", "-"); // hyphens   
        return str;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        // Vietnamese special D handling
        var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        result = result.Replace("Đ", "D").Replace("đ", "d");
        return result;
    }
}
