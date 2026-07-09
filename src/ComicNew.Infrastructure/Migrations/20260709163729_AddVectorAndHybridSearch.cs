using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;
using Pgvector;

#nullable disable

namespace ComicNew.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVectorAndHybridSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "Stories",
                type: "vector",
                nullable: true);

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Stories",
                type: "tsvector",
                nullable: true)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Title", "Description" });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_SearchVector",
                table: "Stories",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.Sql(@"
create or replace function hybrid_search_stories(
  query_text text,
  query_embedding vector,
  match_count int,
  full_text_weight float default 1,
  semantic_weight float default 1,
  rrf_k int default 50
)
returns setof ""Stories""
language sql
as $$
with full_text as (
  select
    ""Id"",
    row_number() over(order by ts_rank_cd(""SearchVector"", websearch_to_tsquery('english', query_text)) desc) as rank_ix
  from
    ""Stories""
  where
    ""SearchVector"" @@ websearch_to_tsquery('english', query_text)
),
semantic as (
  select
    ""Id"",
    row_number() over (order by ""Embedding"" <=> query_embedding) as rank_ix
  from
    ""Stories""
)
select
  ""Stories"".*
from
  full_text
  full outer join semantic
    on full_text.""Id"" = semantic.""Id""
  join ""Stories""
    on coalesce(full_text.""Id"", semantic.""Id"") = ""Stories"".""Id""
order by
  coalesce(1.0 / (rrf_k + full_text.rank_ix), 0.0) * full_text_weight +
  coalesce(1.0 / (rrf_k + semantic.rank_ix), 0.0) * semantic_weight
  desc
limit
  match_count;
$$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop function if exists hybrid_search_stories(text, vector, int, float, float, int);");
            migrationBuilder.DropIndex(
                name: "IX_Stories_SearchVector",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Stories");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");
        }
    }
}
