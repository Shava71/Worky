using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
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
        var filters = BuildStaticFilters(request);

        float[]? queryVector = null;
        if (!string.IsNullOrWhiteSpace(request.AISearch))
            queryVector = await _embeddingService.GetEmbedding(request.AISearch);
        
        SearchRequestDescriptor<ResumeDocument> requestDescriptor = queryVector is not null
            ? CreateHybridSearchDescriptor(
                from,
                request.PageSize,
                filters,
                BuildLexicalQuery(request.AISearch!),
                queryVector)
            : CreateBrowseSearchDescriptor(from, request.PageSize, filters);

        if (queryVector is null)
        {
            ApplySorting(requestDescriptor, request);
        }
        else if (!string.IsNullOrWhiteSpace(request.SortItem))
        {
            _logger.LogInformation(
                "Explicit sort {SortItem} ignored for hybrid resume search to preserve RRF relevance",
                request.SortItem);
        }

        var response = await _client.SearchAsync<ResumeDocument>(requestDescriptor);

        if (!response.IsValidResponse)
        {
            _logger.LogWarning(
                "Hybrid resume search returned invalid response. Falling back to lexical query. Debug: {DebugInformation}",
                response.DebugInformation);

            var fallbackDescriptor = CreateBrowseSearchDescriptor(
                from,
                request.PageSize,
                filters,
                BuildLexicalQuery(request.AISearch!));

            response = await _client.SearchAsync<ResumeDocument>(fallbackDescriptor);
        }

        if (!response.IsValidResponse)
            throw new InvalidOperationException($"Resume search failed: {response.DebugInformation}");

        Guid sessionId = Guid.Empty;
        if(!string.IsNullOrWhiteSpace(request.AISearch))
        {
            sessionId = await SaveSearchSession(request, response);
        }

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

    private Query BuildLexicalQuery(string searchText)
    {
        var should = new List<Query>
        {
            new MultiMatchQuery
            {
                Query = searchText,
                Fields = new Field[]
                {
                    "post^6",
                    "skill^4",
                    "workerFullName^2",
                    "city_search^1.5"
                },
                Type = TextQueryType.BestFields,
                Fuzziness = new Fuzziness("AUTO")
            },
            new NestedQuery
            {
                Path = "activities",
                Query = new MultiMatchQuery
                {
                    Query = searchText,
                    Fields = new Field[]
                    {
                        "activities.direction^2",
                        "activities.type^1.5"
                    },
                    Type = TextQueryType.BestFields,
                    Fuzziness = new Fuzziness("AUTO")
                }
            }
        };

        return new BoolQuery
        {
            Should = should,
            MinimumShouldMatch = 1
        };
    }

    private List<Query> BuildStaticFilters(GetResumesRequest request)
    {
        var must = new List<Query>();
        
        if (request.id.HasValue)
        {
            must.Add(new TermQuery
            {
                Field = "id",
                Value = request.id.Value.ToString()
            });
        }

        if (!string.IsNullOrWhiteSpace(request.city))
        {
            must.Add(new TermQuery
            {
                Field = "city",
                Value = request.city
            });
        }

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
        
        if (!string.IsNullOrWhiteSpace(request.type))
        {
            must.Add(new NestedQuery
            {
                Path = "activities",
                Query = new TermQuery
                {
                    Field = "activities.type",
                    Value = request.type
                }
            });
        }
        
        if (request.direction != null && request.direction.Any())
        {
            var ids = request.direction
                .Where(x => long.TryParse(x, out _))
                .Select(x => FieldValue.Long(long.Parse(x)))
                .ToList();

            must.Add(new NestedQuery
            {
                Path = "activities",
                Query = new TermsQuery
                {
                    Field = "activities.id",
                    Terms = new TermsQueryField(ids)
                }
            });
        }

        return must;
    }
    
    private void ApplySorting(
        SearchRequestDescriptor<ResumeDocument> descriptor,
        GetResumesRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SortItem))
            return;

        var order = request.Order?.ToLower() == "asc"
            ? SortOrder.Asc
            : SortOrder.Desc;

        descriptor.Sort(s =>
        {
            s.Field(new FieldSort
            {
                Field = request.SortItem,
                Order = order
            });
        });
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
