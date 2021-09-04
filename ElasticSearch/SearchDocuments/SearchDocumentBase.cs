using System;
using Nest;

namespace ElasticSearch.SearchDocuments
{
    public class SearchDocumentBase
    {
        public Guid Id { get; set; } // Add types for analyzers
        [Text(Analyzer = "autocomplete")]
        public string Title { get; set; }
    }
}