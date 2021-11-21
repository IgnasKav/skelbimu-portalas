using System;
using Domain;
using ElasticSearch.SearchDocuments.NestedTypes;
using Nest;

namespace ElasticSearch.SearchDocuments
{
    [ElasticsearchType(RelationName = "advertisement")]
    public class AdvertisementSearchDocument: SearchDocumentBase
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        [Text]
        public string Description { get; set; }
        [Text]
        public string State { get; set; }
        [Text]
        public string City { get; set; }
        public int Views { get; set; }
        public decimal Price { get; set; }
        [Nested]
        public AdvertisementCategory Category { get; set; }

        internal static AdvertisementSearchDocument Map(Advertisement advertisement)
        {
            var result = new AdvertisementSearchDocument()
            {
                Id = advertisement.Id,
                Title = advertisement.Title,
                Date = advertisement.Date,
                Description = advertisement.Description,
                State = advertisement.State,
                City = advertisement.City,
                Views = advertisement.Views,
                Price = advertisement.Price,
                Category = AdvertisementCategory.Map(advertisement.Category),
                SearchText = $"{advertisement.Title} {advertisement.Description}"
            };

            return result;
        }
    }
}