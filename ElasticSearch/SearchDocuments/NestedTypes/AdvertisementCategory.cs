using System;
using Domain;
using Nest;

namespace ElasticSearch.SearchDocuments.NestedTypes
{
    public class AdvertisementCategory
    {
        public Guid Id { get; set; }
        [Text]
        public string Name { get; set; }
        public Guid? ParentId { get; set; }

        internal static AdvertisementCategory Map(Category category)
        {
            return new AdvertisementCategory()
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId
            };
        }
    }
}