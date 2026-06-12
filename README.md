# ComicNew Backend

A modern ASP.NET Core backend for the ComicNew platform, built with Clean Architecture principles. This backend manages comic/novel stories, chapters, reading history, and user authentication through Supabase.

## Overview

**Technology Stack:**
- **Framework**: ASP.NET Core 10.0
- **Database**: PostgreSQL (via Npgsql)
- **ORM**: Entity Framework Core 10.0
- **Authentication**: JWT (Supabase)
- **Testing**: xUnit
- **API Documentation**: Swagger/OpenAPI

**Architecture**: Clean Architecture (4-layer separation)

---

## Quick Start

### Prerequisites
- .NET 10.0 SDK
- PostgreSQL database
- Supabase account (for authentication)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd backend-dotnet
   ```

2. **Configure environment variables**
   ```bash
   cp .env.example .env
   ```
   
   Update `.env` with your credentials:
   ```env
   ASPNETCORE_ENVIRONMENT=Development
   PORT=5000
   Supabase__Url=https://your-project.supabase.co
   ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=comic_new;Username=postgres;Password=your-password
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Apply database migrations**
   ```bash
   dotnet ef database update -p src/ComicNew.Infrastructure -s src/ComicNew.Api
   ```

5. **Run the application**
   ```bash
   dotnet watch run --project src/ComicNew.Api/ComicNew.Api.csproj
   ```

   Application runs at: `http://localhost:5000`
   Swagger UI: `http://localhost:5000/swagger`

---

## Project Structure

### Directory Schema

```
backend-dotnet/
├── src/                                    # Source code (4-layer architecture)
│   ├── ComicNew.Domain/                   # Layer 1: Business entities & logic
│   │   ├── Entities/
│   │   │   ├── User.cs                    # User profile (linked to Supabase)
│   │   │   ├── Story.cs                   # Comic/Novel story
│   │   │   ├── Chapter.cs                 # Story chapter with images/content
│   │   │   ├── ReadingHistory.cs          # User reading tracking
│   │   │   └── RefreshToken.cs            # Token refresh management
│   │   ├── Enums/
│   │   │   ├── StoryType.cs               # Comic | Novel
│   │   │   └── StoryStatus.cs             # Ongoing | Completed | Suspended
│   │   ├── Common/
│   │   │   └── BaseEntity.cs              # Base class (Id, CreatedAt, UpdatedAt)
│   │   └── ComicNew.Domain.csproj
│   │
│   ├── ComicNew.Application/              # Layer 2: Use cases & interfaces
│   │   ├── Interfaces/
│   │   │   └── IUserSyncService.cs        # Contract for user sync from Supabase
│   │   └── ComicNew.Application.csproj
│   │
│   ├── ComicNew.Infrastructure/           # Layer 3: Data access & services
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs            # Entity Framework DbContext
│   │   │   └── Configurations/            # EF Core Fluent API configs
│   │   │       ├── UserConfiguration.cs
│   │   │       ├── StoryConfiguration.cs
│   │   │       ├── ChapterConfiguration.cs
│   │   │       ├── ReadingHistoryConfiguration.cs
│   │   │       └── RefreshTokenConfiguration.cs
│   │   ├── Migrations/                    # EF Core migrations
│   │   │   ├── 20260513151408_InitialCreate.*
│   │   │   ├── 20260602135702_AddSupabaseUserId.*
│   │   │   ├── 20260602143000_DropPasswordHashFromUsers.*
│   │   │   └── 20260612094313_AddTypeAndContent.*
│   │   ├── Services/
│   │   │   └── UserSyncService.cs         # Syncs Supabase JWT claims to local DB
│   │   └── ComicNew.Infrastructure.csproj
│   │
│   └── ComicNew.Api/                      # Layer 4: ASP.NET Core entry point
│       ├── Controllers/
│       │   ├── AuthController.cs          # User authentication endpoints
│       │   └── HealthController.cs        # Health check endpoint
│       ├── Program.cs                     # DI, middleware, configuration
│       ├── appsettings.json               # Config template
│       ├── appsettings.Development.json   # Dev-specific config
│       └── ComicNew.Api.csproj
│
├── tests/                                 # Test projects
│   ├── ComicNew.UnitTests/               # Unit tests
│   │   ├── UnitTest1.cs
│   │   └── ComicNew.UnitTests.csproj
│   │
│   └── ComicNew.IntegrationTests/        # Integration tests
│       ├── UnitTest1.cs
│       └── ComicNew.IntegrationTests.csproj
│
├── .github/
│   └── copilot-instructions.md           # AI assistant guidelines
│
├── ComicNew.slnx                         # Solution file
├── Dockerfile                             # Multi-stage Docker build
├── .env                                   # Environment variables (gitignored)
├── .gitignore
└── README.md                              # This file
```

---

## Data Model

### Entity Relationships Diagram

```
User (SupabaseUserId)
├── 1:N → Story (as Author)
├── 1:N → ReadingHistory
├── 1:N → RefreshToken
└── Fields: Id, SupabaseUserId, Email, FullName, AvatarUrl, Bio, Role, LastLoginAt

Story (Comic/Novel)
├── N:1 ← User (Author)
├── 1:N → Chapter
└── Fields: Id, Title, Description, CoverUrl, Tags[], Type, Status, Views, Rating, LastChapterAt

Chapter
├── N:1 ← Story
└── Fields: Id, Title, ChapterNumber, ImageUrls[], Content, Views, PublishedAt

ReadingHistory
├── N:1 ← User
├── N:1 ← Story
└── Fields: Id, UserId, StoryId, ChapterNumber, ReadAt

RefreshToken
├── N:1 ← User
└── Fields: Id, UserId, Token, ExpiresAt, RevokedAt, CreatedByIp, RevokedByIp
```

### Key Enums

| Enum | Values |
|------|--------|
| **StoryType** | Comic, Novel |
| **StoryStatus** | Ongoing, Completed, Suspended |

---

## API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/me` | ✅ Required | Get current user profile |

**Response:**
```json
{
  "userId": "uuid",
  "email": "user@example.com",
  "role": "user",
  "name": "Full Name",
  "avatarUrl": "https://...",
  "claims": [...]
}
```

### Health (`/health`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/health` | ❌ None | Health check endpoint |

---

## Build & Test Commands

### Build & Run
```bash
# Build the solution
dotnet build

# Run in development mode with hot reload
dotnet watch run --project src/ComicNew.Api/ComicNew.Api.csproj

# Run in production mode
dotnet run --project src/ComicNew.Api/ComicNew.Api.csproj --configuration Release
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/ComicNew.UnitTests/ComicNew.UnitTests.csproj
dotnet test tests/ComicNew.IntegrationTests/ComicNew.IntegrationTests.csproj

# Run specific test class
dotnet test --filter "ClassName"

# Run specific test method
dotnet test --filter "ClassName.MethodName"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Database Migrations
```bash
# Create a new migration
dotnet ef migrations add MigrationName -p src/ComicNew.Infrastructure -s src/ComicNew.Api

# Update database to latest migration
dotnet ef database update -p src/ComicNew.Infrastructure -s src/ComicNew.Api

# Revert to previous migration
dotnet ef database update PreviousMigrationName -p src/ComicNew.Infrastructure -s src/ComicNew.Api

# Drop database (dev only)
dotnet ef database drop -p src/ComicNew.Infrastructure -s src/ComicNew.Api
```

---

## Architecture Details

### Clean Architecture Layers

**1. Domain Layer** (`ComicNew.Domain`)
- Contains core business entities and enums
- No external dependencies
- Pure business logic

**2. Application Layer** (`ComicNew.Application`)
- Defines service interfaces/contracts
- Use case orchestration
- Depends on Domain only

**3. Infrastructure Layer** (`ComicNew.Infrastructure`)
- Implements Application interfaces
- Database access via EF Core
- Supabase integration
- Depends on Application and Domain

**4. API Layer** (`ComicNew.Api`)
- ASP.NET Core controllers
- HTTP request handling
- Dependency injection setup
- Depends on all layers

### Key Patterns

**Service Registration (Program.cs)**
```csharp
builder.Services.AddScoped<IUserSyncService, UserSyncService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
```

**Entity Configuration (Fluent API)**
```csharp
// Configurations/ folder uses IEntityTypeConfiguration<T>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasMany(u => u.Stories)
            .WithOne(s => s.Author)
            .HasForeignKey(s => s.AuthorId);
    }
}
```

**JWT Authentication Flow**
1. Client sends JWT token from Supabase
2. `JwtBearerEvents.OnTokenValidated` event triggered
3. `UserSyncService.GetOrCreateUserAsync()` syncs user data from JWT claims
4. User created/updated in local PostgreSQL database
5. Controller accesses user via `User` property (ClaimsPrincipal)

---

## Configuration

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Supabase": {
    "Url": "https://your-project.supabase.co"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=comic_new;..."
  }
}
```

### CORS Policy
- **Development**: `localhost:3000`, `localhost:*`, `127.0.0.1`
- **Production**: `comic-new.vercel.app`, `comic-new-*.vercel.app` (Vercel preview builds)

### Environment Variables
Set via `.env` file or system environment:
```env
ASPNETCORE_ENVIRONMENT=Development
Supabase__Url=https://your-project.supabase.co
ConnectionStrings__DefaultConnection=Host=...;
```

---

## Code Style & Conventions

### Naming
- **File-scoped namespaces**: `namespace ComicNew.Domain.Entities;`
- **JSON property naming**: PascalCase (no camelCase conversion)
- **Null safety**: Enabled globally; use `string?` and `null!` assertions

### Entity Configuration
- Use Fluent API exclusively (no data annotations)
- All configurations in `Infrastructure/Persistence/Configurations/`
- Inherit from `IEntityTypeConfiguration<T>`

### Service Lifetime
- **Scoped**: Most services (created per request)
- Use constructor injection in controllers

---

## Docker Deployment

### Build Docker Image
```bash
docker build -t comic-new-backend .
```

### Run Docker Container
```bash
docker run -d \
  -e Supabase__Url=https://your-project.supabase.co \
  -e ConnectionStrings__DefaultConnection="Host=db-host;..." \
  -p 8080:8080 \
  comic-new-backend
```

The Dockerfile uses multi-stage build for optimization:
1. **Build stage**: Restores packages, builds and publishes
2. **Runtime stage**: Uses lightweight ASP.NET runtime image

---

## Development Workflow

### Adding a New Feature

1. **Define domain entity** in `Domain/Entities/`
   ```csharp
   public class Story : BaseEntity { ... }
   ```

2. **Create EF configuration** in `Infrastructure/Persistence/Configurations/`
   ```csharp
   public class StoryConfiguration : IEntityTypeConfiguration<Story> { ... }
   ```

3. **Create migration**
   ```bash
   dotnet ef migrations add AddStory -p src/ComicNew.Infrastructure -s src/ComicNew.Api
   ```

4. **Create service interface** in `Application/Interfaces/`
   ```csharp
   public interface IStoryService { ... }
   ```

5. **Implement service** in `Infrastructure/Services/`
   ```csharp
   public class StoryService : IStoryService { ... }
   ```

6. **Register service** in `Program.cs`
   ```csharp
   builder.Services.AddScoped<IStoryService, StoryService>();
   ```

7. **Create controller** in `Api/Controllers/`
   ```csharp
   [ApiController]
   [Route("api/stories")]
   public class StoriesController : ControllerBase { ... }
   ```

8. **Add tests** in `tests/`

### Running Hot Reload
```bash
dotnet watch run --project src/ComicNew.Api/ComicNew.Api.csproj
```

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Database connection fails | Check `.env` credentials and PostgreSQL is running |
| JWT validation fails | Verify Supabase URL and token audience is "authenticated" |
| Migrations not applied | Run `dotnet ef database update -p src/ComicNew.Infrastructure -s src/ComicNew.Api` |
| Port already in use | Change PORT in `.env` or use `--urls` parameter |
| Swagger not loading | Ensure `ASPNETCORE_ENVIRONMENT=Development` |

---

## Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Docs](https://docs.microsoft.com/ef/core)
- [Supabase Authentication](https://supabase.com/docs/guides/auth)
- [PostgreSQL Documentation](https://www.postgresql.org/docs)
- [Clean Architecture by Robert Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## Contributing

See `.github/copilot-instructions.md` for development guidelines and best practices.

---

## License

[Add your license here]
