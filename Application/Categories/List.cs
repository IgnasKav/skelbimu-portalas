using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Advertisements;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Categories
{
    public class List
    {
        public class Query : IRequest<List<CategoryDto>>
        {
        }

        public class Handler : IRequestHandler<Query, List<CategoryDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<CategoryDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var categories = await _context.Categories
                    .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return categories;
            }
        }
    }
}