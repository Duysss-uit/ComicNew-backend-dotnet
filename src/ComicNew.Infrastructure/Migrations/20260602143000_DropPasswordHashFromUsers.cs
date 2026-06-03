using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicNew.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropPasswordHashFromUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "SupabaseUserId" uuid;
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_SupabaseUserId" ON "Users" ("SupabaseUserId");
                ALTER TABLE "Users" DROP COLUMN IF EXISTS "PasswordHash";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS "IX_Users_SupabaseUserId";
                ALTER TABLE "Users" DROP COLUMN IF EXISTS "SupabaseUserId";
                ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "PasswordHash" character varying(500) NOT NULL DEFAULT '';
                """);
        }
    }
}
