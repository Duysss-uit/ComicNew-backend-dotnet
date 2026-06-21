using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ComicNew.Application.Interfaces;
using ComicNew.Infrastructure.Persistence;
using ComicNew.Infrastructure.Services;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// CORS configuration to allow requests from the frontend application
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(IsAllowedCorsOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserSyncService, UserSyncService>();
var supabaseUrl = builder.Configuration["Supabase:Url"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{supabaseUrl}/auth/v1";
        options.MapInboundClaims = false;
        
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
builder.Services.AddSingleton<Supabase.Client>(provider =>
{
    var url = builder.Configuration["Supabase:Url"] 
        ?? throw new InvalidOperationException("Supabase:Url is missing");
    var key = builder.Configuration["Supabase:Key"] 
        ?? throw new InvalidOperationException("Supabase:Key is missing");
    
    return new Supabase.Client(url, key, new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = false
    });
});
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IChapterUploadService, ChapterUploadService>();
builder.Services.AddScoped<IStorageService, SupabaseStorageService>();
builder.Services.AddScoped<IUserSyncService, UserSyncService>();
builder.Services.AddScoped<IReadingHistoryService, ReadingHistoryService>();
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            message = "An unexpected server error occurred."
        });
    });
});

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

static bool IsAllowedCorsOrigin(string origin)
{
    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
    {
        return false;
    }

    if (uri.Scheme == Uri.UriSchemeHttp && uri.Host is "localhost" or "127.0.0.1")
    {
        return true;
    }

    return uri.Scheme == Uri.UriSchemeHttps
        && (uri.Host == "comic-new.vercel.app"
            || (uri.Host.StartsWith("comic-new-", StringComparison.OrdinalIgnoreCase)
                && uri.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)));
}
