using System.Threading;
using System.Threading.Tasks;
using Domain;
using ElasticSearch;
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
                _context.Advertisements.Add(request.Advertisement);
                
                await _context.SaveChangesAsync();
                
                return Unit.Value;
            }
        }
    }
}