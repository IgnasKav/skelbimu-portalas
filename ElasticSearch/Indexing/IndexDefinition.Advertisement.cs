using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.SearchDocuments;
using Microsoft.EntityFrameworkCore;
using Nest;
using Persistence;

namespace ElasticSearch.Indexing
{
    public class AdvertisementIndexDefinition: IndexDefinition
    {
        public override string Name => "advertisement";
        internal override async Task<int> IndexAsync(IElasticClient client, DataContext context, List<Guid> ids)
        {
            var advertisements = await context.Advertisements
                .Where(a => ids.Contains(a.Id))
                .Include(a => a.Category).ToListAsync();
            var documentList = new List<AdvertisementSearchDocument>();
            foreach (var advertisement in advertisements)
            {
                documentList.Add(AdvertisementSearchDocument.Map(advertisement));
            }

            return await PerfomIndexing(client, documentList);
        }

        public override async Task<CreateIndexResponse> Create(IElasticClient client)
        {
            return await client.Indices.CreateAsync(Name,
                i => i.Settings(CommonIndexDescriptor)
                    .Map<AdvertisementSearchDocument>(map => map.AutoMap()));
        }
    }
}