using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElasticSearch;
using ElasticSearch.Indexing;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.BackgroundJobs;

public class AdvertisementBackgroundJob
{
    public class Query : IRequest
    {
        public ElasticBackgroundOperation ElasticBackgroundOperation { get; set; }
    }

    public class Handler : IRequestHandler<Query>
    {
        private readonly DataContext _context;
        private readonly IElasticSearchService _es;
        private readonly IBackgroundJobClient _backgroundJobs;

        public Handler(DataContext context, IElasticSearchService es, IBackgroundJobClient backgroundJobs)
        {
            _context = context;
            _es = es;
            _backgroundJobs = backgroundJobs;
        }

        public Task<Unit> Handle(Query request, CancellationToken cancellationToken)
        {
            switch (request.ElasticBackgroundOperation)
            {
                case ElasticBackgroundOperation.Delete:
                     _backgroundJobs.Enqueue(() => DeleteAdvertisementIndex());
                    break;
                case ElasticBackgroundOperation.Create:
                    _backgroundJobs.Enqueue(() => CreateAdvertisementIndex());
                    break;
                case ElasticBackgroundOperation.Reindex:
                    _backgroundJobs.Enqueue(() => ReindexAdvertisements());
                    break;
            }
            
            return Task.FromResult(Unit.Value);
        }

        public async Task DeleteAdvertisementIndex()
        {
            await _es.DeleteIndex(IndexDefinition.Advertisement);
        }
        
        public async Task CreateAdvertisementIndex()
        {
            await _es.CreateIndex(IndexDefinition.Advertisement);
        }
        
        public async Task ReindexAdvertisements()
        {
            var advertisements = await _context.Advertisements.ToListAsync();
            var advertisementIds = advertisements.Select(a => a.Id).ToList();
            await _es.Reindex(IndexDefinition.Advertisement, advertisementIds);
        }
    }
}