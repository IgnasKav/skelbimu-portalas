using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Categories
{
    public class Edit
    {
        public class Command : IRequest
        {
            public Category Category { get; set; }
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
                Category category = await _context.Categories.FindAsync(request.Category.Id);

                category.Name = request.Category.Name;

                await _context.SaveChangesAsync();
                
                return Unit.Value;
            }
        }
    }
}