using Elastic.Clients.Elasticsearch;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.DAL.Repositories.Implementations;

public class ElasticRepository<T> : IElasticRepository<T> where T : class
{
    protected readonly ElasticsearchClient _client;
    protected readonly string _indexName;

    public ElasticRepository(ElasticsearchClient client, string indexName)
    {
        _client = client;
        _indexName = indexName.ToLower();
    }

    public async Task IndexAsync(string id, T document)
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
}