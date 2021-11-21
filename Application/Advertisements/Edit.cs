using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using ElasticSearch;
using ElasticSearch.Indexing;
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

                if (advertisement == null)
                {
                    throw new Exception("Advertisement does not exist");
                }

                if (request.Advertisement.Views < advertisement.Views)
                {
                    throw new Exception("Cant decrease advertisement views");
                }

                _mapper.Map(request.Advertisement, advertisement);
                
                await _context.SaveChangesAsync();
                await _es.Reindex(IndexDefinition.Advertisement, new List<Guid> {advertisement.Id});

                return Unit.Value;
            }
        }
    }
}