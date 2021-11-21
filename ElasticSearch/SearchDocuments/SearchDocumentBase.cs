using System;
using Nest;

namespace ElasticSearch.SearchDocuments
{
    public abstract class SearchDocumentBase
    {
        public Guid Id { get; set; } // Add types for analyzers
        [Text]
        public string SearchText { get; set; }
    }
}