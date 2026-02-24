using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Microsoft.Extensions.Logging;
using SearchService.BLL.Services.Interfaces;
using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.DAL.Repositories.Implementations;

public class ResumeElasticRepository 
    : ElasticRepository<ResumeDocument>, IResumeElasticRepository
{
    private readonly IEmbeddingService _embeddingService;
    private readonly SearchDbContext _context;
    private readonly ILogger<ResumeElasticRepository> _logger;

    public ResumeElasticRepository(
        ElasticsearchClient client,
        IEmbeddingService embeddingService,
        SearchDbContext context,
        ILogger<ResumeElasticRepository> logger)
        : base(client, "resumes")
    {
        _embeddingService = embeddingService;
        _context = context;
        _logger = logger;
    }

    public override async Task IndexAsync(string id, ResumeDocument document)
    {
        string text = $"{document.post ?? ""} {document.skill ?? ""}".Trim();

        if (string.IsNullOrWhiteSpace(text))
            text = document.post ?? "";

        document.vector = await _embeddingService.GetEmbedding(text);

        await base.IndexAsync(id, document);
    }

    public async Task<Contract.SearchResponse<ResumeSearchResultDto>> SearchAsync(GetResumesRequest request)
    {
        var from = (request.Page - 1) * request.PageSize;

        var mustFilters = BuildStaticFilters(request);

        float[]? queryVector = null;

        if (!string.IsNullOrWhiteSpace(request.AISearch))
            queryVector = await _embeddingService.GetEmbedding(request.AISearch);

        Query finalQuery;

        if (queryVector != null)
        {
            finalQuery = new ScriptScoreQuery
            {
                Query = new BoolQuery
                {
                    Must = mustFilters,
                    Should = new List<Query>
                    {
                        new MultiMatchQuery
                        {
                            Query = request.AISearch,
                            Fields = new Field[]
                            {
                                "post^4",
                                "skill^3"
                            },
                            Type = TextQueryType.BestFields,
                            Fuzziness = new Fuzziness("AUTO")
                        }
                    },
                    MinimumShouldMatch = 1
                },
                Script = new Script
                {
                    Source = """
                        double bm25 = _score;
                        double cosine = cosineSimilarity(params.vector, 'vector');
                        return params.alpha * bm25 + params.beta * cosine;
                    """,
                    Params = new Dictionary<string, object>
                    {
                        { "alpha", 0.6 },
                        { "beta", 0.4 },
                        { "vector", queryVector }
                    }
                }
            };
        }
        else
        {
            finalQuery = new BoolQuery
            {
                Must = mustFilters
            };
        }

        var response = await _client.SearchAsync<ResumeDocument>(s => s
            .Index(_indexName)
            .From(from)
            .Size(request.PageSize)
            .Query(finalQuery)
            .TrackTotalHits(true)
            .Source(src => src
                .Filter(f => f.Excludes(e => e.vector)))
        );

        var sessionId = await SaveSearchSession(request, response);

        return new Contract.SearchResponse<ResumeSearchResultDto>
        {
            Total = response.Total,
            Page = request.Page,
            PageSize = request.PageSize,
            SessionId = sessionId,
            Items = response.Hits.Select(h => new ResumeSearchResultDto
            {
                Document = h.Source!,
                Score = h.Score ?? 0
            }).ToList()
        };
    }

    private List<Query> BuildStaticFilters(GetResumesRequest request)
    {
        var must = new List<Query>();

        if (request.min_experience.HasValue || request.max_experience.HasValue)
            must.Add(new NumberRangeQuery
            {
                Field = "experience",
                Gte = request.min_experience,
                Lte = request.max_experience
            });

        if (request.education.HasValue)
            must.Add(new TermQuery
            {
                Field = "educationId",
                Value = request.education.Value
            });

        if (request.min_wantedSalary.HasValue)
            must.Add(new NumberRangeQuery
            {
                Field = "wantedSalary",
                Gte = request.min_wantedSalary.Value
            });
        
        if (request.max_wantedSalary.HasValue)
            must.Add(new NumberRangeQuery
            {
                Field = "wantedSalary",
                Lte = request.max_wantedSalary.Value
            });
        
        

        return must;
    }

    private async Task<Guid> SaveSearchSession(
        GetResumesRequest request,
        Elastic.Clients.Elasticsearch.SearchResponse<ResumeDocument> response)
    {
        var session = new SearchSession
        {
            Id = Guid.NewGuid(),
            Query = request.AISearch ?? "",
            UserId = request.UserId
        };

        int position = 1;

        foreach (var hit in response.Hits)
        {
            session.Impressions.Add(new SearchImpression
            {
                Id = Guid.NewGuid(),
                DocumentId = Guid.Parse(hit.Id!),
                DocumentType = "resume",
                Position = position++,
                Clicked = false
            });
        }

        _context.SearchSessions.Add(session);
        await _context.SaveChangesAsync();

        return session.Id;
    }
}