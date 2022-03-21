using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using ElasticSearch;
using ElasticSearch.Indexing;
using MediatR;
using Persistence;

namespace Application.Advertisements
{
    public class Create
    {
        public class Command : IRequest
        {
            public Advertisement Advertisement { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IElasticSearchService _es;
            
            public Handler(DataContext context, IElasticSearchService es)
            {
                _context = context;
                _es = es;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Advertisement.Title))
                {
                    throw new Exception("Advertisement must have title");
                }

                if (request.Advertisement.Views > 0)
                {
                    throw new Exception("Initial views must be 0");
                }

                var advertisement = _context.Advertisements.Add(request.Advertisement);

                await _context.SaveChangesAsync();
                await _es.Reindex(IndexDefinition.Advertisement, new List<Guid> {advertisement.Entity.Id});
                
                return Unit.Value;
            }
        }
    }
}