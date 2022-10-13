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
    public class Delete
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
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
                Advertisement advertisement = await _context.Advertisements.FindAsync(request.Id);
                await _es.DeleteDocument(IndexDefinition.Advertisement, advertisement.Id);
                
                _context.Advertisements.Remove(advertisement);
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}