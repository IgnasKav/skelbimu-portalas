using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Categories
{
    public class Create
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
                Category category = request.Category;
                if (category.ParentId == Guid.Empty)
                {
                    category.ParentId = null;
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                
                return Unit.Value;
            }
        }
    }
}