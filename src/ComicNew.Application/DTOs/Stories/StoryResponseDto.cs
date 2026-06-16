using ComicNew.Application.DTOs.Users;
using ComicNew.Domain.Entities;
using ComicNew.Domain.Enums;
namespace ComicNew.Application.DTOs.Stories
{
    public class StoryResponseDto
    {
        public Guid Id { get; set; }
        public int Views { get; set; }
        public double Rating { get; set; }
        public DateTime LastChapterAt { get; set; }
        public AuthorDto? Author { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public string[] Tags { get; set; } = [];
        public string Status { get; set; } = StoryStatus.Ongoing.ToString();
        public StoryType Type { get; set; } = StoryType.Comic;
        AuthorDto authorDto {get; set;} = new AuthorDto();
    }
}