using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Advertisements
{
    public class List
    {
        public class Query : IRequest<List<Advertisement>>
        {

        }

        public class Handler : IRequestHandler<Query, List<Advertisement>>
        {
            private readonly DataContext _context;
            
            public Handler(DataContext context)
            {
                _context = context;
            }
            
            public async Task<List<Advertisement>> Handle(Query request, CancellationToken cancellationToken)
            { 
                return await _context.Advertisements.ToListAsync(cancellationToken);
            }
        }
    }
}