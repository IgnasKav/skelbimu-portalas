using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Advertisements
{
    public class List
    {
        public class Query : IRequest<List<AdvertisementDto>>
        {

        }

        public class Handler : IRequestHandler<Query, List<AdvertisementDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            
            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<List<AdvertisementDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var advertisements = await _context.Advertisements
                    .ProjectTo<AdvertisementDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                
                return advertisements;
            }
        }
    }
}