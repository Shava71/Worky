using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SeachService.DAL.DTO;
using SearchService.Contract;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.DAL.Repositories.Implementations;

public class VacancyElasticRepository : ElasticRepository<VacancyDocument>, IVacancyElasticRepository
{
    public VacancyElasticRepository(ElasticsearchClient client)
        : base(client, "vacancies") { }
    
    public async Task<IReadOnlyCollection<(VacancyDocument doc, double score)>> SearchAsync(GetVacanciesRequest request)
    {
        var must = new List<Query>();
        var should = new List<Query>();

        // ID
        if (request.id.HasValue)
            must.Add(new TermQuery { Field = "id", Value = request.id.Value.ToString() });

        // Опыт
        if (request.min_experience.HasValue || request.max_experience.HasValue)
            must.Add(new NumberRangeQuery
            {
                Field = "experience",
                Gte = request.min_experience,
                Lte = request.max_experience
            });

        // Образование
        if (request.education.HasValue)
            must.Add(new TermQuery { Field = "educationId", Value = request.education.Value });
        
        // // Город (ищем в компании)
        // if (!string.IsNullOrWhiteSpace(request.city))
        //     must.Add(new MatchQuery { Field = "company.city", Query = request.city });

        // Зарплата: ищем пересечение диапазонов
        if (request.min_wantedSalary.HasValue || request.max_wantedSalary.HasValue)
        {
            var salaryClauses = new List<Query>();

            if (request.min_wantedSalary.HasValue)
                salaryClauses.Add(new NumberRangeQuery { Field = "maxSalary", Gte = request.min_wantedSalary.Value });

            if (request.max_wantedSalary.HasValue)
                salaryClauses.Add(new NumberRangeQuery { Field = "minSalary", Lte = request.max_wantedSalary.Value });

            must.Add(new BoolQuery { Should = salaryClauses, MinimumShouldMatch = 1 });
        }

        // Тип деятельности
        if (!string.IsNullOrWhiteSpace(request.type))
            must.Add(new TermQuery { Field = "activities.type.keyword", Value = request.type });

        // Направления
        if (request.direction?.Count > 0)
            must.Add(new TermsQuery
            {
                Field = "activities.direction.keyword",
                Terms = new TermsQueryField(request.direction.Select(FieldValue.String).ToList())
            });

        // геолокация:  только если оба параметра переданы
        if (!string.IsNullOrWhiteSpace(request.latitude) && 
            !string.IsNullOrWhiteSpace(request.longitude) &&
            double.TryParse(request.latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lat) &&
            double.TryParse(request.longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lon))
        {
            must.Add(new GeoDistanceQuery
            {
                Field = "company.location",
                Distance = "50km", // можно вынести в параметр позже
                Location = new LatLonGeoLocation { Lat = lat, Lon = lon }
            });
        }

        // Умный поиск (AISearch)
        if (!string.IsNullOrWhiteSpace(request.AISearch))
        {
            should.Add(new MultiMatchQuery
            {
                Query = request.AISearch,
                Fields = new Field[]
                {
                    "post^4",
                    "description^2",
                    "company.name^3",
                    "company.city",
                    "activities.direction",
                    "activities.type",
                    "workFormat_name",
                    "workFour_name"
                },
                Type = TextQueryType.MostFields,
                Fuzziness = new Fuzziness(1)
            });
        }
        // // Обычный поиск по строке (если нет AISearch)
        // else if (!string.IsNullOrWhiteSpace(request.search))
        // {
        //     should.Add(new MultiMatchQuery
        //     {
        //         Query = request.search,
        //         Fields = new Field[] { "post^4", "description^2", "company.name^3", "company.city" },
        //         Type = TextQueryType.MostFields,
        //         Fuzziness = Fuzziness.Auto
        //     });
        // }

        var query = new BoolQuery
        {
            Must = must.Count > 0 ? must : null,
            Should = should.Count > 0 ? should : null,
            MinimumShouldMatch = should.Count > 0 ? 1 : 0
        };

        // Сортировка
        var sortOptions = new List<SortOptions>();

        // Если переданы координаты — сортируем по расстоянию в первую очередь
        if (!string.IsNullOrWhiteSpace(request.latitude) &&
            !string.IsNullOrWhiteSpace(request.longitude) &&
            double.TryParse(request.latitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var sortLat) &&
            double.TryParse(request.longitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var sortLon))
        {
            sortOptions.Add(new SortOptions
            {
                GeoDistance = new GeoDistanceSort
                {
                    Field = "company.location",
                    Location =  [new LatLonGeoLocation { Lat = sortLat, Lon = sortLon }] ,
                    Order = SortOrder.Asc,
                    Unit = DistanceUnit.Kilometers,
                    Mode = SortMode.Min
                }
            });
        }

        // Пользовательская сортировка
        if (!string.IsNullOrWhiteSpace(request.SortItem))
        {
            var order = string.Equals(request.Order, "desc", StringComparison.OrdinalIgnoreCase)
                ? SortOrder.Desc
                : SortOrder.Asc;

            sortOptions.Add(new SortOptions
            {
                Field = new FieldSort { Field = request.SortItem, Order = order }
            });
        }
        else
        {
            // По умолчанию — по дате поступления
            sortOptions.Add(new SortOptions
            {
                Field = new FieldSort { Field = "incomeDate", Order = SortOrder.Desc }
            });
        }

        var response = await _client.SearchAsync<VacancyDocument>(s => s
            .Index(_indexName)
            .Query(query)
            .Sort(sortOptions.ToArray())
            .Size(100)
            .TrackTotalHits(true)
        );

        if (!response.IsValidResponse)
            throw new Exception($"Elasticsearch error: {response.DebugInformation}");

        return response.Hits
            .Select(h => (h.Source!, h.Score ?? 0.0))
            .ToList();
    }
}