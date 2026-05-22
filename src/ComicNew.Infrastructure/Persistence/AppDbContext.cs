using ComicNew.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace ComicNew.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Story> Stories { get; set; } = null!;
    public DbSet<Chapter> Chapters { get; set; } = null!;
    public DbSet<ReadingHistory> ReadingHistories { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.EnableSensitiveDataLogging();
    }
}