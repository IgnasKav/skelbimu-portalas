using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
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
            
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                Advertisement advertisement = await _context.Advertisements.FindAsync(request.Id);
                
                _context.Advertisements.Remove(advertisement);
                
                await _context.SaveChangesAsync();
                
                return Unit.Value;
            }
        }
    }
}