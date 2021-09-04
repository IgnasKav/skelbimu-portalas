using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using ElasticSearch;
using MediatR;
using Persistence;

namespace Application.Advertisements
{
    public class Edit
    {
        public class Command : IRequest
        {
            public Advertisement Advertisement { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IElasticSearchService _es;
            
            public Handler(DataContext context, IMapper mapper, IElasticSearchService es)
            {
                _context = context;
                _mapper = mapper;
                _es = es;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                Advertisement advertisement = await _context.Advertisements.FindAsync(request.Advertisement.Id);

                _mapper.Map(request.Advertisement, advertisement);
                
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}