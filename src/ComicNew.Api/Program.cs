using Microsoft.EntityFrameworkCore;
using ComicNew.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
