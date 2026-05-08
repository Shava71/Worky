using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.DAL.Repositories.Implementations;

public class ElasticRepository<T> : IElasticRepository<T> where T : class
{
    protected readonly ElasticsearchClient _client;
    protected readonly string _indexName;
    protected const int DefaultRrfRankConstant = 60;
    protected const int DefaultMinimumRankWindow = 100;
    protected const int DefaultKnnCandidateMultiplier = 4;
    protected const int DefaultMaxRescoreWindow = 200;
    protected const double DefaultOriginalQueryWeight = 0.35;
    protected const double DefaultRescoreQueryWeight = 1.4;

    public ElasticRepository(ElasticsearchClient client, string indexName)
    {
        _client = client;
        _indexName = indexName.ToLower();
    }

    public virtual async Task IndexAsync(string id, T document)
    {
        var response = await _client.IndexAsync(document, x => x.Index(_indexName).Id(id).Refresh(Refresh.True));

        if (!response.IsValidResponse)
            throw new Exception(response.DebugInformation);
    }

    public async Task UpdateAsync(string id, T document)
    {
        var response = await _client.UpdateAsync<T, object>(
            index: _indexName,
            id: id,
            descriptor => descriptor.Doc(document) 
        );

        if (!response.IsValidResponse)
            throw new Exception(response.DebugInformation);
    }

    public async Task DeleteAsync(string id)
    {
        var response = await _client.DeleteAsync<T>(id, d => d.Index(_indexName).Refresh(Refresh.True));

        if (!response.IsValidResponse)
            throw new Exception(response.DebugInformation);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var response = await _client.GetAsync<T>(id, g => g.Index(_indexName));

        return response.Source;
    }

    protected SearchRequestDescriptor<T> CreateBrowseSearchDescriptor(
        int from,
        int size,
        IReadOnlyCollection<Query> filters)
    {
        return CreateBrowseSearchDescriptor(from, size, filters, null);
    }

    protected SearchRequestDescriptor<T> CreateBrowseSearchDescriptor(
        int from,
        int size,
        IReadOnlyCollection<Query> filters,
        Query? lexicalQuery)
    {
        Query query = BuildFilteredQuery(filters, lexicalQuery);

        return new SearchRequestDescriptor<T>()
            .Indices(_indexName)
            .From(from)
            .Size(size)
            .Query(query)
            .TrackTotalHits(true)
            .Source(src => src.Filter(f => f.Excludes("vector")));
    }

    protected SearchRequestDescriptor<T> CreateHybridSearchDescriptor(
        int from,
        int size,
        IReadOnlyCollection<Query> filters,
        Query lexicalQuery,
        float[] queryVector)
    {
        var requestedWindow = Math.Max(from + size, size);
        var rankWindowSize = Math.Max(DefaultMinimumRankWindow, requestedWindow * 4);
        var knnK = rankWindowSize;
        var numCandidates = Math.Max(DefaultMinimumRankWindow, knnK * DefaultKnnCandidateMultiplier);
        var rescoreWindowSize = Math.Min(rankWindowSize, DefaultMaxRescoreWindow);

        return new SearchRequestDescriptor<T>()
            .Indices(_indexName)
            .From(from)
            .Size(size)
            .Query(BuildFilteredQuery(filters, lexicalQuery))
            .Knn([
                new KnnSearch
                {
                    Field = "vector",
                    QueryVector = queryVector,
                    K = knnK,
                    NumCandidates = numCandidates,
                    Filter = filters.ToList()
                }
            ])
            .Rank(new Rank
            {
                Rrf = new RrfRank
                {
                    RankConstant = DefaultRrfRankConstant,
                    RankWindowSize = rankWindowSize
                }
            })
            .Rescore(
            [
                new Rescore
                {
                    WindowSize = rescoreWindowSize,
                    Query = new RescoreQuery
                    {
                        Query = BuildSemanticRescoreQuery(queryVector),
                        QueryWeight = DefaultOriginalQueryWeight,
                        RescoreQueryWeight = DefaultRescoreQueryWeight,
                        ScoreMode = ScoreMode.Total
                    }
                }
            ])
            .TrackTotalHits(true)
            .Source(src => src.Filter(f => f.Excludes("vector")));
    }

    protected static Query BuildFilteredQuery(
        IReadOnlyCollection<Query> filters,
        Query? lexicalQuery)
    {
        if (filters.Count == 0)
            return lexicalQuery ?? new MatchAllQuery();

        return lexicalQuery is null
            ? new BoolQuery { Filter = filters.ToList() }
            : new BoolQuery
            {
                Must = [lexicalQuery],
                Filter = filters.ToList()
            };
    }

    protected static ScriptScoreQuery BuildSemanticRescoreQuery(float[] queryVector) =>
        new()
        {
            Query = new MatchAllQuery(),
            Script = new Script
            {
                Source = "cosineSimilarity(params.vector, 'vector') + 1.0",
                Params = new Dictionary<string, object>
                {
                    { "vector", queryVector }
                }
            }
        };
}
