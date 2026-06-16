namespace ComicNew.Application.DTOs.Users
{
    public class AuthorDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}