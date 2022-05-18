using System.Collections.Generic;
using ElasticSearch.SearchDocuments;

namespace ElasticSearch
{
    public class ElasticSearchRequest
    {
        public int From { get; set; }
        public int Size { get; set; }
        public string Query { get; set; }
        public string? UserId { get; set; }
        public List<ElasticCategoriesFilter>? CategoryFilters { get; set; }
    }

    public class ElasticCategoriesFilter
    {
        public string CategoryFilter { get; set; }
        public string CategoryId { get; set; }
    }

    public class ElasticSearchOptions
    {
        public string Url { get; set; }
    }
    
    public class ElasticSearchResults<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
        public string Query { get; set; }
        public string DebugInformation { get; set; }

        public ElasticSearchResults()
        {
            Items = new List<T>();
        }
    }
}