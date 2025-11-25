using System.Globalization;
using System.Text.RegularExpressions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using SeachService.DAL.DTO;
using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Interfaces;
using SearchService.ML;


namespace SearchService.DAL.Repositories.Implementations;

public class VacancyElasticRepository : ElasticRepository<VacancyDocument>, IVacancyElasticRepository
{
    private readonly ILogger<VacancyElasticRepository> _logger;
    private readonly SbertEmbeddingService _embeddingService;
    public VacancyElasticRepository(ElasticsearchClient client, ILogger<VacancyElasticRepository> logger, SbertEmbeddingService embeddingService)
        : base(client, "vacancies") { _logger = logger; _embeddingService = embeddingService; }

    public override async Task IndexAsync(string id, VacancyDocument document)
    {
        string text = $"{document.post ?? ""} {document.description ?? ""}".Trim();

        if (string.IsNullOrWhiteSpace(text))
            text = document.post ?? "";
        document.vector = _embeddingService.GetEmbedding(text);
        
        await base.IndexAsync(id, document);
    }

    public async Task<IReadOnlyCollection<VacancySearchResultDto>> SearchAsync(GetVacanciesRequest request)
    {
        _logger.LogInformation("Vacancy search started with request: {@request}", request);

        var must = new List<Query>();   // Только жёсткие фильтры (не влияют на score)
        var should = new List<Query>();  // Здесь будет текстовый поиск — он даёт score!

        try
        {
            // ЖЁСТКИЕ ФИЛЬТРЫ (must) — НЕ влияют на score

            if (request.id.HasValue)
                must.Add(new TermQuery { Field = "id", Value = request.id.Value.ToString() });

            if (request.min_experience.HasValue || request.max_experience.HasValue)
                must.Add(new NumberRangeQuery
                {
                    Field = "experience",
                    Gte = request.min_experience,
                    Lte = request.max_experience
                });

            if (request.education.HasValue)
                must.Add(new TermQuery { Field = "educationId", Value = request.education.Value });

            // Зарплата — сложный фильтр
            if (request.min_wantedSalary.HasValue || request.max_wantedSalary.HasValue)
            {
                var salaryClauses = new List<Query>();

                if (request.min_wantedSalary.HasValue)
                    salaryClauses.Add(new NumberRangeQuery { Field = "maxSalary", Gte = request.min_wantedSalary.Value });

                if (request.max_wantedSalary.HasValue)
                    salaryClauses.Add(new NumberRangeQuery { Field = "minSalary", Lte = request.max_wantedSalary.Value });

                must.Add(new BoolQuery
                {
                    Should = salaryClauses,
                    MinimumShouldMatch = 1
                });
            }

            if (!string.IsNullOrWhiteSpace(request.type))
                must.Add(new TermQuery { Field = "activities.type.keyword", Value = request.type });

            if (request.direction?.Count > 0)
                must.Add(new TermsQuery
                {
                    Field = "activities.direction.keyword",
                    Terms = new TermsQueryField(request.direction.Select(FieldValue.String).ToList())
                });

            // Геофильтр
            if (!string.IsNullOrWhiteSpace(request.latitude) &&
                !string.IsNullOrWhiteSpace(request.longitude) &&
                double.TryParse(request.latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(request.longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lon))
            {
                must.Add(new GeoDistanceQuery
                {
                    Field = "location",
                    Distance = "50km",
                    Location = new LatLonGeoLocation { Lat = lat, Lon = lon }
                });
            }

            // ТЕКСТОВЫЙ ПОИСК — ОСНОВНОЙ ИСТОЧНИК SCORE 
            if (!string.IsNullOrWhiteSpace(request.AISearch))
            {
                var textSearchQuery = new MultiMatchQuery
                {
                    Query = request.AISearch.Trim(),
                    Fields = new Field[]
                    {
                        "post^5",                    // Должность — самый важный
                        "description^3",             // Описание
                        "company.name^2",
                        "activities.direction^2",
                        "activities.type^2",
                        "workHour",
                        "workFormat"
                    },
                    Type = TextQueryType.BestFields,  // Лучше для точного совпадения в одном поле
                    Fuzziness = new Fuzziness("AUTO"),
                    Operator = Operator.Or,
                    // MinimumShouldMatch = "75%" // можно включить для строгих запросов
                };

                should.Add(textSearchQuery);
            }

            // Если вообще ничего не указано — ищем по всему 
            Query finalQuery;

            if (must.Count > 0 || should.Count > 0)
            {
                finalQuery = new BoolQuery
                {
                    Must = must.Count > 0 ? must : null,
                    Should = should.Count > 0 ? should : null,
                    MinimumShouldMatch = should.Count > 0 ? 1 : 0   //  Заставляем should влиять
                };
            }
            else
            {
                finalQuery = new MatchAllQuery();
            }

            // === СОРТИРОВКА ===
            var sortOptions = new List<SortOptions>();

            // Гео-сортировка по расстоянию
            if (!string.IsNullOrWhiteSpace(request.latitude) &&
                !string.IsNullOrWhiteSpace(request.longitude) &&
                double.TryParse(request.latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var sortLat) &&
                double.TryParse(request.longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var sortLon))
            {
                sortOptions.Add(new GeoDistanceSort
                {
                    Field = "location",
                    Location = new[] { GeoLocation.LatitudeLongitude(new LatLonGeoLocation(sortLat, sortLon)) },
                    Order = SortOrder.Asc,
                    Unit = DistanceUnit.Kilometers,
                    Mode = SortMode.Min
                });
            }

            // Пользовательская сортировка
            if (!string.IsNullOrWhiteSpace(request.SortItem))
            {
                var order = string.Equals(request.Order, "desc", StringComparison.OrdinalIgnoreCase)
                    ? SortOrder.Desc
                    : SortOrder.Asc;

                sortOptions.Add(new FieldSort { Field = request.SortItem, Order = order });
            }
            else if (string.IsNullOrWhiteSpace(request.latitude) || string.IsNullOrWhiteSpace(request.longitude))
            {
                // По умолчанию — сначала свежие + по релевантности
                sortOptions.Add(new FieldSort { Field = "_score", Order = SortOrder.Desc });
                sortOptions.Add(new FieldSort { Field = "incomeDate", Order = SortOrder.Desc });
            }

            // === ВЫПОЛНЕНИЕ ЗАПРОСА ===
            var searchResponse = await _client.SearchAsync<VacancyDocument>(s => s
                .Index(_indexName)
                .Query(finalQuery)
                .Sort(sortOptions)
                .Size(100)
                .TrackTotalHits(true)
                .Source(true) // если не нужно всё — можно отключить
            );

            if (!searchResponse.IsValidResponse)
            {
                _logger.LogError("Elasticsearch error: {error}", searchResponse.DebugInformation);
                throw new Exception($"Elasticsearch error: {searchResponse.DebugInformation}");
            }

            _logger.LogInformation("Search success. Total hits: {total}, Returned: {count}",
                searchResponse.Total, searchResponse.Documents.Count);

            foreach (var hit in searchResponse.Hits)
            {
                _logger.LogInformation("Hit ID: {id} | Score: {score:F4} | Post: {post}",
                    hit.Id, hit.Score, hit.Source?.post);
            }

            var result = searchResponse.Hits
                .Where(h => h.Source != null)
                .Select(h => new VacancySearchResultDto
                {
                    Document = h.Source!,
                    Score = h.Score ?? 0.0
                })
                .ToList();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during vacancy search");
            throw;
        }
    }
    
    public async Task<IReadOnlyCollection<VacancySearchResultDto>> SearchLexicalAsync(string query)
    {
        var should = new List<Query>();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var textSearchQuery = new MultiMatchQuery
            {
                Query = query.Trim(),
                Fields = new Field[]
                {
                    "post^5",
                    "description^3",
                    "company.name^2",
                    "activities.direction^2",
                    "activities.type^2",
                    "workHour",
                    "workFormat"
                },
                Type = TextQueryType.BestFields,
                Fuzziness = new Fuzziness("AUTO"),
                Operator = Operator.Or
            };

            should.Add(textSearchQuery);
        }


        Query? finalQuery;
        if (should.Count > 0)
        {
            finalQuery = new BoolQuery
            {
                Should = should,
                MinimumShouldMatch = 1
            };
        }
        else
        {
            finalQuery = new MatchAllQuery();
        }

        var response = await _client.SearchAsync<VacancyDocument>(s => s
            .Index(_indexName)
            .Query(finalQuery)
            // .Size(100)
            .TrackTotalHits(true)
        );

        if (!response.IsValidResponse)
            throw new Exception(response.DebugInformation);

        return response.Hits
            .Select(h => new VacancySearchResultDto
            {
                Document = h.Source!,
                Score = h.Score ?? 0
            })
            .ToList();
    }
    
    public async Task<IReadOnlyCollection<VacancySearchResultDto>> SearchSemanticAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Array.Empty<VacancySearchResultDto>();

        // Получаем вектор для запроса
        float[] queryVector = _embeddingService.GetEmbedding(query);

        // Выполняем чистый KNN поиск
        var response = await _client.SearchAsync<VacancyDocument>(s => s
            .Index(_indexName)
            .Knn(knn => knn
                    .Field(f => f.vector)
                    .QueryVector(queryVector)
                    .K(50)               // сколько документов вернуть
                    .NumCandidates(384)  // сколько кандидатов просканировать
            )
            .MinScore(0.5f)
            .Size(50)
        );

        if (!response.IsValidResponse)
            throw new Exception(response.DebugInformation);

        // Формируем результат
        return response.Hits
            .Select(h => new VacancySearchResultDto
            {
                Document = h.Source!,
                Score = h.Score ?? 0
            })
            .ToList();
    }
    
    public async Task<IReadOnlyCollection<VacancySearchResultDto>> SearchHybridAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            // просто вернем MatchAll через лексический поиск
            return await SearchLexicalAsync(query);
        }

        // --- 1. Запускаем лексический поиск ---
        var lexicalResults = await SearchLexicalAsync(query);

        // --- 2. Запускаем семантический поиск ---
        var semanticResults = await SearchSemanticAsync(query);

        // Если оба пустые — вернуть пусто
        if (lexicalResults.Count == 0 && semanticResults.Count == 0)
            return Array.Empty<VacancySearchResultDto>();


        // --- 3. RRF fusion ---
        const int k = 60;

        // Ранжируем
        var lexRanked = lexicalResults
            .Select((r, i) => new { r.Document, Score = r.Score, Rank = i + 1 });

        var semRanked = semanticResults
            .Select((r, i) => new { r.Document, Score = r.Score, Rank = i + 1 });

        // Объединяем
        var combined = lexRanked.Concat(semRanked)
            .GroupBy(x => x.Document.id)
            .Select(g => new
            {
                Doc = g.First().Document,
                Score = g.Sum(x => 1.0 / (k + x.Rank)) // RRF
            })
            .OrderByDescending(x => x.Score)
            .Take(50)
            .Select(x => new VacancySearchResultDto
            {
                Document = x.Doc,
                Score = (float)x.Score
            })
            .ToList();

        return combined;
    }
    
    public async Task<IReadOnlyCollection<VacancySearchResultDto>> SearchTwoStageAsync(string query)
    {
        // 1) Получаем вектор запроса
        float[] queryVector = _embeddingService.GetEmbedding(query);
        
        // var boolQuery = new BoolQuery
        // {
        //     Should = new List<Query>()
        // };
        //
        // if (!string.IsNullOrWhiteSpace(query))
        // {
        //     MultiMatchQuery queryBm = new MultiMatchQuery
        //     {
        //         Query = query.Trim(),
        //         Fields = new Field[]
        //         {
        //             "post^5",
        //             "description^3",
        //             "company.name^2",
        //             "activities.direction^2",
        //             "activities.type^2",
        //             "workHour",
        //             "workFormat"
        //         },
        //         Type = TextQueryType.BestFields,
        //         Fuzziness = new Fuzziness("AUTO"),
        //         Operator = Operator.Or,
        //     };
        //     
        //     boolQuery.Should.Add(queryBm);
        // }
        //
        // // 2) Первый этап — быстрый BM25 поиск (кандидаты)
        // var bm25 = await _client.SearchAsync<VacancyDocument>(s => s
        //         .Index(_indexName)
        //         .Query(q => q.Bool(boolQuery))
        //         .Size(300) // количество кандидатов
        // );
        var bm25 = await SearchLexicalAsync(query);
        Console.WriteLine("BM25 found: " + bm25.Count);


        var candidates = bm25
            .Where(d => d.Document.vector != null && d.Document.vector.Length == queryVector.Length)
            .ToList();
        
        Console.WriteLine("With vectors: " + candidates.Count);

        // 3) Rerank — пересчёт score локально (косинусная близость)
        foreach (var doc in candidates)
        {
            doc.Document.TempScore = CosineSimilarity(queryVector, doc.Document.vector!);
        }

        // 4) Возвращаем top-N документов
        return candidates
            .OrderByDescending(c => c.Document.TempScore)
            .Take(50)
            .Select(c => new VacancySearchResultDto
            {
                Document = c.Document,
                Score = c.Document.TempScore
            })
            .ToList();
    }
    
    private double CosineSimilarity(float[] v1, float[] v2)
    {
        if (v1 == null || v2 == null) return 0;
        if (v1.Length != v2.Length) return 0;

        double dot = 0, mag1 = 0, mag2 = 0;

        for (int i = 0; i < v1.Length; i++)
        {
            dot += v1[i] * v2[i];
            mag1 += v1[i] * v1[i];
            mag2 += v2[i] * v2[i];
        }

        if (mag1 == 0 || mag2 == 0) return 0;

        return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
    }
}