using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.Indexing;
using Elasticsearch.Net;
using ElasticSearch.SearchDocuments;
using Nest;
using Persistence;

namespace ElasticSearch
{
    public interface IElasticSearchService
    {
        IElasticClient Client { get; }
        public Task<int> CreateIndex(IndexDefinition index);
        public Task<int> Reindex(IndexDefinition index, List<Guid> ids = null);
        public Task<int> DeleteDocuments(IndexDefinition index, List<Guid> ids = null);
        public Task<ElasticSearchResults<T>> Search<T>(IndexDefinition index, string query)
            where T : SearchDocumentBase;
        public Task<int> DeleteIndex(IndexDefinition index);
        public Task<bool> IndexExists(IndexDefinition index);
    }

    public class ElasticSearchService: IElasticSearchService
    {
        private readonly ElasticSearchOptions _options;
        private readonly DataContext _context;
        public IElasticClient Client { get; }

        public ElasticSearchService(ElasticSearchOptions options, DataContext context)
        {
            _options = options;
            _context = context;

            var connectionPool = new SingleNodeConnectionPool(new Uri(options.Url));
            var settings = new ConnectionSettings(connectionPool);

            Client = new ElasticClient(settings);
        }

        public async Task<int> CreateIndex(IndexDefinition index)
        {
            await index.Create(Client);

            return 0;
        }

        //returns count of processed items
        public async Task<int> Reindex(IndexDefinition index, List<Guid> ids = null)
        {
            //find maximum batch size
            const int batchSize = 100;
            if (ids != null || ids.Any() || ids.Count < batchSize)
            {
                return await index.IndexAsync(Client, _context, ids);
            }

            return 0;
        }

        public async Task<int> DeleteDocuments(IndexDefinition index, List<Guid> ids = null)
        {
            const int batchSize = 100;
            if (ids != null || ids.Any() || ids.Count < batchSize)
            {
                return await index.DeleteDocuments(Client, ids);
            }

            return 0;
        }

        //pakeist query
        public async Task<ElasticSearchResults<T>> Search<T>(IndexDefinition index, string query)
        where T : SearchDocumentBase
        { 
            var searchResponse = await Client.SearchAsync<T>(
                s => 
                    s
                        .Index(index.Name)
                        .Query(
                    q => q.Match(
                        m => m.Field(
                            f => f.SearchText)
                            .Query(query))));
            var results = new ElasticSearchResults<T>()
            {
                Total = (int)searchResponse.Total,
                Items = searchResponse.Documents.ToList(),
                DebugInformation = searchResponse.DebugInformation,
                Query = query
            };
                
            return results;
        }

        public async Task<int> DeleteIndex(IndexDefinition index)
        {
            if (await IndexExists(index))
            {
                await Client.Indices.DeleteAsync(index.Name);
            }

            return 0;
        }

        public async Task<bool> IndexExists(IndexDefinition index)
        {
            return (await Client.Indices.ExistsAsync(index.Name)).Exists;
        }
    }
}