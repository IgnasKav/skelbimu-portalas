using System;
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
    public class Details
    {
        public class Query : IRequest<AdvertisementDto>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, AdvertisementDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<AdvertisementDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var advertisement = await _context.Advertisements
                .ProjectTo<AdvertisementDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

                return advertisement;
            }
        }
    }
}