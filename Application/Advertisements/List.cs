using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Advertisements
{
    public class List
    {
        public class Query : IRequest<List<AdvertisementDto>>
        {
            public Guid Id { get; set;} = Guid.Empty;
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

                if(request.Id != Guid.Empty)
                {
                   advertisements = advertisements.FindAll(x => x.Category.id == request.Id).ToList();
                }

                return advertisements;
            }
        }
    }
}