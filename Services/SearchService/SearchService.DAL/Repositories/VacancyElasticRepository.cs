using System.Globalization;
using System.Text.RegularExpressions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using SeachService.DAL.DTO;
using SearchService.BLL.Services.Interfaces;
using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Interfaces;
using SearchService.ML;


namespace SearchService.DAL.Repositories.Implementations;

public class VacancyElasticRepository : ElasticRepository<VacancyDocument>, IVacancyElasticRepository
{
    private readonly ILogger<VacancyElasticRepository> _logger;
    // private readonly SbertEmbeddingService _embeddingService;
    private readonly IEmbeddingService _embeddingService;
    private readonly SearchDbContext _context;

    public VacancyElasticRepository(ElasticsearchClient client, ILogger<VacancyElasticRepository> logger,
        // SbertEmbeddingService embeddingService
        IEmbeddingService embeddingService,
        SearchDbContext context
    )
        : base(client, "vacancies")
    {
        _logger = logger; 
        _embeddingService = embeddingService;
        _context = context;
    }

    public override async Task IndexAsync(string id, VacancyDocument document)
    {
        string text = $"{document.post ?? ""} {document.description ?? ""}".Trim();

        if (string.IsNullOrWhiteSpace(text))
            text = document.post ?? "";
        document.vector = await _embeddingService.GetEmbedding(text);
        
        await base.IndexAsync(id, document);
    }

    public async Task<Contract.SearchResponse<VacancySearchResultDto>> SearchAsync(GetVacanciesRequest request)
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
                    Filter = mustFilters,
                    Should = new List<Query>
                    {
                        new MultiMatchQuery
                        {
                            Query = request.AISearch,
                            Fields = new Field[]
                            {
                                "post^5",
                                "description^3",
                                "company.name^2",
                                "activities.direction^2",
                                "activities.type^2"
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

        SearchRequestDescriptor<VacancyDocument> searchDescriptor = new SearchRequestDescriptor<VacancyDocument>()
            .Index(_indexName)
            .From(from)
            .Size(request.PageSize)
            .Query(finalQuery)
            .TrackTotalHits(true)
            .Source(src => src
                .Filter(f => f.Excludes(e => e.vector)));
        ApplySorting(searchDescriptor, request);
        var response = await _client.SearchAsync<VacancyDocument>(searchDescriptor);
        
        Guid sessionId = Guid.Empty;
        if(!string.IsNullOrWhiteSpace(request.AISearch))
        {
            sessionId = await SaveSearchSession(request, response);
        }
       

        return new Contract.SearchResponse<VacancySearchResultDto>
        {
            Total = response.Total,
            Page = request.Page,
            PageSize = request.PageSize,
            SessionId = sessionId,
            Items = response.Hits
                .Select(h => new VacancySearchResultDto
                {
                    Document = h.Source!,
                    Score = h.Score ?? 0
                })
                .ToList()
        };
    }

    private List<Query> BuildStaticFilters(GetVacanciesRequest request)
        {
            var must = new List<Query>();

            if (request.id.HasValue)
                must.Add(new TermQuery { Field = "id", Value = request.id.Value.ToString() });

            if (request.min_experience.HasValue || request.max_experience.HasValue)
                must.Add(new NumberRangeQuery
                {
                    Field = "experience",
                    Gte = request.min_experience,
                    Lte = request.max_experience
                });
            
            if (request.min_wantedSalary.HasValue)
                must.Add(new NumberRangeQuery
                {
                    Field = "minSalary",
                    Gte = request.min_wantedSalary,
                });
            
            if (request.max_wantedSalary.HasValue)
                must.Add(new NumberRangeQuery
                {
                    Field = "maxSalary",
                    Lte = request.max_wantedSalary,
                });
            
            if (request.income_date.HasValue)
                must.Add(new DateRangeQuery
                {
                    Field = "incomeDate",
                    Gte = request.income_date.Value
                });

            if (request.education.HasValue)
                must.Add(new TermQuery { Field = "educationId", Value = request.education.Value });
            
            if (!string.IsNullOrWhiteSpace(request.latitude) &&
                !string.IsNullOrWhiteSpace(request.longitude) &&
                double.TryParse(request.latitude, out var lat) &&
                double.TryParse(request.longitude, out var lon))
            {
                must.Add(new GeoDistanceQuery
                {
                    Field = "location",
                    Distance = "50km",
                    Location = new LatLonGeoLocation
                    {
                        Lat = lat,
                        Lon = lon
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

            return must;
        }
    
        private void ApplySorting(
            SearchRequestDescriptor<VacancyDocument> descriptor,
            GetVacanciesRequest request)
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
        
        private async Task<Guid> SaveSearchSession(GetVacanciesRequest request, Elastic.Clients.Elasticsearch.SearchResponse<VacancyDocument> response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
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
                    DocumentType = "vacancy",
                    Position = position++,
                    Clicked = false
                });
            }

            _context.SearchSessions.Add(session);
            await _context.SaveChangesAsync();

            return session.Id;
        }
}
