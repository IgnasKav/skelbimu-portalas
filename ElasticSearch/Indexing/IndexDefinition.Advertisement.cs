using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Elasticsearch.Net;
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
                var shallowCategory = await context.Categories.FindAsync(advertisement.CategoryId);
                var category = await GetCategoryRecursively(shallowCategory.Id, context);
                var imageEntities = await context.AdvertisementImage.Where(i =>
                    i.AdvertisementId == advertisement.Id).ToListAsync();
                var imageUrl = "";

                if (imageEntities.Count > 0)
                {
                    imageUrl = imageEntities[0].ImagePath;
                }

                advertisement.Category = category;
                documentList.Add(AdvertisementSearchDocument.Map(advertisement, imageUrl));
            }

            return await PerformIndexing(client, documentList);
        }
        
        internal override async Task<int> IndexAsync(IElasticClient client, DataContext context, Guid id)
        {
            var advertisement = await context.Advertisements
                .FirstOrDefaultAsync(x => x.Id == id);
            var shallowCategory = await context.Categories.FindAsync(advertisement.CategoryId);
            var category = await GetCategoryRecursively(shallowCategory.Id, context);
            var imageEntities = await context.AdvertisementImage.Where(i =>
                i.AdvertisementId == advertisement.Id).ToListAsync();
            var imageUrl = "";

            if (imageEntities.Count > 0)
            {
                imageUrl = imageEntities[0].ImagePath;
            }
            var advertisementSearchDocument = AdvertisementSearchDocument.Map(advertisement, imageUrl);
            advertisement.Category = category;

            return await PerformIndexing(client, advertisementSearchDocument);
        }

        internal override async Task<int> DeleteDocument(IElasticClient client, DataContext context, Guid id)
        {
            await client.DeleteAsync<AdvertisementSearchDocument>(id,
                b => b.Refresh(Refresh.True));
            
            return 0;
        }

        public async Task<Category> GetCategoryRecursively(Guid categoryId, DataContext context)
        {
            var category = await context.Categories.FindAsync(categoryId);

            if (category.ParentId.HasValue && category.ParentId != Guid.Empty)
            {
                var parent = await GetCategoryRecursively(category.ParentId.Value, context);
                category.Parent = parent;
            }

            return category;
        }

        public override async Task<CreateIndexResponse> Create(IElasticClient client)
        {
            return await client.Indices.CreateAsync(Name,
                i => i.Settings(CommonIndexDescriptor)
                    .Map<AdvertisementSearchDocument>(map => map.AutoMap()
                        .Properties(p => p
                            .Text(t => t
                                .Name(n => n.CategoryFilter)
                                .Analyzer("whitespace")))));
        }
    }
}