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
        public Task<int> Reindex(IndexDefinition index, Guid id);
        public Task<ElasticSearchResults<T>> Search<T>(ElasticSearchRequest query, List<ISort> sortQuery = null, List<QueryContainer> filters = null, List<QueryContainer> categoriesFilter = null)
            where T : SearchDocumentBase;
        public Task<int> DeleteIndex(IndexDefinition index);
        public Task<int> DeleteDocument(IndexDefinition index, Guid advertisementId);
        public Task<bool> IndexExists(IndexDefinition index);
    }

    public class ElasticSearchService: IElasticSearchService
    {
        private int elasticPageSize = 9;
        private readonly DataContext _context;
        public IElasticClient Client { get; }

        public ElasticSearchService(ElasticSearchOptions options, DataContext context)
        {
            _context = context;

            var connectionPool = new SingleNodeConnectionPool(new Uri(options.Url));
            var settings = new ConnectionSettings(connectionPool)
                .DefaultMappingFor<AdvertisementSearchDocument>(m => m
                .IndexName(IndexDefinition.Advertisement.Name));

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

        public async Task<int> Reindex(IndexDefinition index, Guid id)
        {
            return await index.IndexAsync(Client, _context, id);
        }

        //pakeist query
        //aggregations tinka tik skelbimams
        public async Task<ElasticSearchResults<T>> Search<T>(ElasticSearchRequest request, List<ISort> sortQuery = null, List<QueryContainer> filters = null, List<QueryContainer> categoriesFilter = null)
        where T : SearchDocumentBase
        {
            var boolQuery = new BoolQuery
            {
                Must = new List<QueryContainer>
                {
                    new QueryStringQuery()
                    {
                        Fields = Infer.Field<AdvertisementSearchDocument>(f => f.SearchText),
                        Query = request.Query
                    }
                },
            };

            if (categoriesFilter != null && categoriesFilter.Count > 0)
            {
                boolQuery.Should = categoriesFilter;
                boolQuery.MinimumShouldMatch = 1;
            }

            if (filters != null && filters.Count > 0)
            {
                boolQuery.Filter = filters;
            }

            var aggregationContainer =
                (IAggregationContainer)new AggregationContainerDescriptor<AdvertisementSearchDocument>().Terms("state",
                    x => x.Field(doc => doc.State));

            var from = 0;
            if (request.Page != 0)
            {
                from = ((request.Page - 1) * elasticPageSize) + elasticPageSize;
            }

            var searchRequest = new SearchRequest<AdvertisementSearchDocument>
            {
                Query = boolQuery,
                Aggregations = aggregationContainer.Aggregations,
                From = from,
                Size = elasticPageSize,
                Sort= sortQuery
            };
            
            var searchResponse = await Client.SearchAsync<T>(searchRequest);
            var results = new ElasticSearchResults<T>()
            {
                Total = (int)searchResponse.Total,
                Items = searchResponse.Documents.ToList(),
                DebugInformation = searchResponse.DebugInformation,
                Query = request.Query
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

        public async Task<int> DeleteDocument(IndexDefinition index, Guid advertisementId)
        {
            return await index.DeleteDocument(Client, _context, advertisementId);
        }

        public async Task<bool> IndexExists(IndexDefinition index)
        {
            return (await Client.Indices.ExistsAsync(index.Name)).Exists;
        }
    }
}