using System.Globalization;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using SeachService.DAL.DTO;
using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.DAL.Repositories.Implementations;

public class VacancyElasticRepository : ElasticRepository<VacancyDocument>, IVacancyElasticRepository
{
    private readonly ILogger<VacancyElasticRepository> _logger;
    public VacancyElasticRepository(ElasticsearchClient client, ILogger<VacancyElasticRepository> logger)
        : base(client, "vacancies") { _logger = logger; }
    
    public async Task<IReadOnlyCollection<VacancySearchResultDto>> SearchAsync(GetVacanciesRequest request)
{
    _logger.LogInformation("Vacancy search started with request: {@request}", request);

    var must = new List<Query>();   // Только жёсткие фильтры (не влияют на score)
    var should = new List<Query>();  // Здесь будет текстовый поиск — он даёт score!

    try
    {
        // === ЖЁСТКИЕ ФИЛЬТРЫ (must) — НЕ влияют на score ===

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

        // === ТЕКСТОВЫЙ ПОИСК — ОСНОВНОЙ ИСТОЧНИК SCORE ===
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

        // === Если вообще ничего не указано — ищем по всему ===
        Query finalQuery;

        if (must.Count > 0 || should.Count > 0)
        {
            finalQuery = new BoolQuery
            {
                Must = must.Count > 0 ? must : null,
                Should = should.Count > 0 ? should : null,
                MinimumShouldMatch = should.Count > 0 ? 1 : 0   // Ключевое! Заставляем should влиять
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
}