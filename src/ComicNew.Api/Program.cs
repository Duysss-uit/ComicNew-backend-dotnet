using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ComicNew.Application.Interfaces;
using ComicNew.Infrastructure.Persistence;
using ComicNew.Infrastructure.Services;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173",
                            "comic-new.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserSyncService, UserSyncService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var supabaseUrl = builder.Configuration["Supabase:Url"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{supabaseUrl}/auth/v1";
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{supabaseUrl}/auth/v1",
            
            ValidateAudience = true,
            ValidAudience = "authenticated",
            
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.RequireHttpsMetadata = builder.Environment.IsProduction();

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                if (context.Principal is null)
                {
                    return;
                }

                var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("UserSync");

                try
                {
                    var userSync = context.HttpContext.RequestServices.GetRequiredService<IUserSyncService>();
                    var user = await userSync.GetOrCreateUserAsync(context.Principal, context.HttpContext.RequestAborted);
                    logger.LogInformation("User sync completed for {Email} ({SupabaseUserId})", user.Email, user.SupabaseUserId);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "User sync failed during token validation.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void LoadDotEnv()
{
    var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (!File.Exists(envPath))
    {
        envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
    }

    if (!File.Exists(envPath))
    {
        return;
    }

    foreach (var line in File.ReadAllLines(envPath))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
        {
            continue;
        }

        var separatorIndex = trimmed.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = trimmed[..separatorIndex].Trim();
        var value = trimmed[(separatorIndex + 1)..].Trim().Trim('"');

        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
