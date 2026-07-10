using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicNew.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHybridSearchWithTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
    s.""Id"",
    row_number() over(order by 
      ts_rank_cd(s.""SearchVector"", websearch_to_tsquery('english', query_text)) +
      coalesce(
        (select ts_rank_cd(to_tsvector('english', string_agg(t.""Name"", ' ')), websearch_to_tsquery('english', query_text))
         from ""StoryTags"" st
         join ""Tags"" t on st.""TagsId"" = t.""Id""
         where st.""StoriesId"" = s.""Id""
        ), 0.0) 
      desc) as rank_ix
  from
    ""Stories"" s
  where
    s.""SearchVector"" @@ websearch_to_tsquery('english', query_text) OR
    exists (
      select 1 from ""StoryTags"" st
      join ""Tags"" t on st.""TagsId"" = t.""Id""
      where st.""StoriesId"" = s.""Id"" 
      and to_tsvector('english', t.""Name"") @@ websearch_to_tsquery('english', query_text)
    )
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
    }
}
