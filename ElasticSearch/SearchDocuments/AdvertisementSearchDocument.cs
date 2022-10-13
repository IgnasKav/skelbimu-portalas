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
        [Text(Analyzer = "whitespace", Name = nameof(CategoryFilter))]
        public string CategoryFilter { get; set; }
        public DateTime Date { get; set; }
        [Text]
        public string Description { get; set; }
        public AdvertisementState State { get; set; }
        [Text]
        public string City { get; set; }
        public int Views { get; set; }
        public decimal Price { get; set; }
        [Nested]
        public AdvertisementCategory Category { get; set; }
        public Guid OwnerId { get; set; }
        public string ImageUrl { get; set; }

        internal static AdvertisementSearchDocument Map(Advertisement advertisement, string imageUrl)
        {
            var result = new AdvertisementSearchDocument()
            {
                Id = advertisement.Id,
                Title = advertisement.Title,
                CategoryFilter = CreateCategoryFilter(advertisement.Category),
                Date = advertisement.Date,
                Description = advertisement.Description,
                State = advertisement.State,
                City = advertisement.City,
                Views = advertisement.Views,
                Price = advertisement.Price,
                OwnerId = advertisement.OwnerId,
                Category = AdvertisementCategory.Map(advertisement.Category),
                SearchText = $"{advertisement.Title} {advertisement.Description}",
                ImageUrl = imageUrl
            };

            return result;
        }

        public static string CreateCategoryFilter(Category category)
        {
            var currentElement = category;
            string categoryFilter = "";

            while (currentElement.Parent != null)
            {
                categoryFilter += $"{currentElement.Name} ";
                currentElement = currentElement.Parent;
                
            }
            categoryFilter += $"{currentElement.Name} ";

            return categoryFilter;
        }
    }
}