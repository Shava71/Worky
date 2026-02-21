using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using SearchService.Contract;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.DAL.Repositories.Implementations;

public class ResumeElasticRepository : ElasticRepository<ResumeDocument>, IResumeElasticRepository
{
    public ResumeElasticRepository(ElasticsearchClient client)
        : base(client, "resumes") { }
    
    public async Task<IReadOnlyCollection<(ResumeDocument doc, double score)>> SearchAsync(GetResumesRequest request)
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

    // Образование — ищем по id
    if (request.education.HasValue)
        must.Add(new TermQuery { Field = "educationId", Value = request.education.Value });

    // Город
    if (!string.IsNullOrWhiteSpace(request.city))
        must.Add(new MatchQuery { Field = "city", Query = request.city });

    // Желаемая зарплата
    if (request.min_wantedSalary.HasValue || request.max_wantedSalary.HasValue)
        must.Add(new NumberRangeQuery
        {
            Field = "wantedSalary",
            Gte = request.min_wantedSalary,
            Lte = request.max_wantedSalary
        });

    // Тип деятельности (type) — вложенное поле
    if (!string.IsNullOrWhiteSpace(request.type))
        must.Add(new TermQuery
        {
            Field = "activities.type.keyword",
            Value = request.type
        });

    // Направления (direction) — вложенное поле
    if (request.direction?.Count > 0)
        must.Add(new TermsQuery
        {
            Field = "activities.direction.keyword",
            Terms = new TermsQueryField(request.direction.Select(FieldValue.String).ToList())
        });

    // УМНЫЙ ПОИСК — только по реально существующим полям
    if (!string.IsNullOrWhiteSpace(request.AISearch))
    {
        should.Add(new MultiMatchQuery
        {
            Query = request.AISearch,
            Fields = new Field[]
            {
                "post^4",                          // должность — важнее всего
                "skill^3",                         // навыки
                "worker.firstName",               // имя
                "worker.secondName",              // фамилия
                "worker.surname",                  // отчество
                "activities.direction^2",         // направление
                "activities.type^2",               // тип деятельности
                "city"
            },
            Type = TextQueryType.MostFields,
            Fuzziness = new Fuzziness(1)
        });
    }

    var query = new BoolQuery
    {
        Must = must.Count > 0 ? must : null,
        Should = should.Count > 0 ? should : null,
        MinimumShouldMatch = should.Count > 0 ? 1 : 0
    };

    // Сортировка
    var sortOptions = new List<SortOptions>();
    if (!string.IsNullOrWhiteSpace(request.SortItem))
    {
        var order = request.Order?.Trim().ToLower() is "desc" or "descending"
            ? SortOrder.Desc
            : SortOrder.Asc;

        sortOptions.Add(new SortOptions
        {
            Field = new FieldSort { Field = request.SortItem, Order = order }
        });
    }
    else
    {
        sortOptions.Add(new SortOptions
        {
            Field = new FieldSort { Field = "incomeDate", Order = SortOrder.Desc }
        });
    }

    var response = await _client.SearchAsync<ResumeDocument>(s => s
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