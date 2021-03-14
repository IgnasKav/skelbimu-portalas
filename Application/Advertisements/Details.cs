using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Advertisements
{
    public class Details
    {
        public class Query: IRequest<Advertisement>
        {
            public Guid Id { get; set; }

        }

        public class Handler : IRequestHandler<Query, Advertisement>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Advertisement> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Advertisements.FindAsync(request.Id);
            }
        }
    }
}