using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Advertisements.WatchLater
{
    public class AddToWatchLater
    {
        public class Command : IRequest
        {
            public WatchLaterDto WatchLater { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;

            public Handler(IHttpContextAccessor httpContextAccessor, DataContext context, UserManager<User> userManager)
            {
                _httpContextAccessor = httpContextAccessor;
                _context = context;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUserClaim = _httpContextAccessor.HttpContext?.User;
                if (currentUserClaim == null)
                {
                    throw new Exception("Current user not found");
                }
        
                var currentUserId = new Guid(_userManager.GetUserId(currentUserClaim));

                var watchLater = await _context.WatchLater
                    .FirstOrDefaultAsync(item => item.AdvertisementId == request.WatchLater.AdvertisementId && item.UserId == currentUserId, cancellationToken);

                if (watchLater != null)
                {
                    _context.WatchLater.Remove(watchLater);
                    await _context.SaveChangesAsync();
                    return Unit.Value;
                }

                watchLater = new Domain.WatchLater
                {
                    Id = new Guid(),
                    UserId = currentUserId,
                    AdvertisementId = request.WatchLater.AdvertisementId
                };
        
                _context.WatchLater.Add(watchLater);
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}