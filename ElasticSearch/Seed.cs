using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.Indexing;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace ElasticSearch
{
    public class ElasticSeed
    {
        public static async Task ElasticStartup(DataContext context, IElasticSearchService es)
        {
            var elasticIndices = IndexDefinition.All;
            var advertisements = await context.Advertisements.ToListAsync();
            var advertisementIds = advertisements.Select(a => a.Id).ToList();

            foreach (var index in elasticIndices)
            {
                // await es.DeleteIndex(index);
                var indexExists = await es.IndexExists(index);
                if (!indexExists)
                {
                    await es.CreateIndex(index);
                    await es.Reindex(index, advertisementIds);
                }
            }
        }
    }
}