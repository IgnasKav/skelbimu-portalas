using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.SearchDocuments;
using Nest;
using Persistence;

namespace ElasticSearch.Indexing
{
    public abstract class IndexDefinition
    {
        public abstract string Name { get; }
        public abstract Task<CreateIndexResponse> Create(IElasticClient client);
        internal abstract Task<int> IndexAsync(IElasticClient client, DataContext context, List<Guid> ids);
        // internal abstract Task<int> IndexAsync<T>(IElasticClient client, List<T> documents) where T: class;
        public static AdvertisementIndexDefinition Advertisement => new AdvertisementIndexDefinition();
        public static IEnumerable<IndexDefinition> All
        {
            get
            {
                yield return Advertisement;
            }
        }

        protected async Task<int> PerfomIndexing<T>(IElasticClient client, List<T> documents)
            where T : SearchDocumentBase
        {
            if (documents.Any())
            {
                var asyncBulkIndexResponse = await client.BulkAsync(b => b
                    .Index(Name)
                    .IndexMany(documents)
                );
                
                if (asyncBulkIndexResponse.Errors)
                {
                    // Handle error...
                }
                
                return asyncBulkIndexResponse.Items.Count;
            }

            return 0;
        }


        protected static IPromise<IIndexSettings> CommonIndexDescriptor(IndexSettingsDescriptor descriptor)
        {
            return descriptor.NumberOfReplicas(0).NumberOfShards(1).Analysis(InitCommonAnalyzers);
        }

        //update analysers
        private static IAnalysis InitCommonAnalyzers(AnalysisDescriptor analysis)
        {
            return analysis.Analyzers(a => a
                    .Custom("autocomplete", cc => cc
                        .Filters("lowercase", "asciifolding", "edgeNGram")
                        .CharFilters("char_mapping_filter")
                        .Tokenizer("whitespace")
                    )
                )
                .TokenFilters(f => f
                    .EdgeNGram("edgeNGram", e => e
                        .MinGram(1)
                        .MaxGram(15)
                    )
                )
                .CharFilters(f => f.Mapping("char_mapping_filter", fd =>
                    fd.Mappings(
                        "\" => ",
                        "\' => ",
                        "“ => ",
                        "” => ",
                        "„ => ",
                        "` => "
                    )
                ));
        }
    }
}