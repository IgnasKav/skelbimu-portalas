using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IAuthorizationService _authorizationService;
            private readonly UserManager<User> _userManager;

            public Handler(IHttpContextAccessor httpContextAccessor, DataContext context, IMapper mapper, IAuthorizationService authorizationService, UserManager<User> userManager)
            {
                _httpContextAccessor = httpContextAccessor;
                _context = context;
                _mapper = mapper;
                _authorizationService = authorizationService;
                _userManager = userManager;
            }

            public async Task<AdvertisementDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var currentUserClaim = _httpContextAccessor.HttpContext?.User;
                if (currentUserClaim == null)
                {
                    throw new Exception("Current user not found");
                }
                
                var advertisement = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                await _context.Entry(advertisement).Reference(x => x.Category).LoadAsync(cancellationToken);
                
                var advertisementDto = _mapper.Map<Advertisement, AdvertisementDto>(advertisement);
                var currentUserId = new Guid(_userManager.GetUserId(currentUserClaim));
                var watchLater = await _context.WatchLater
                    .FirstOrDefaultAsync(item => item.AdvertisementId == advertisement.Id && item.UserId == currentUserId, cancellationToken);
                var imageEntity = await _context.AdvertisementImage.Where(i =>
                    i.AdvertisementId == advertisement.Id).ToListAsync();

                advertisementDto.WatchLater = watchLater != null;

                advertisementDto.ImageUrl = imageEntity.Count > 0 ? imageEntity[0].ImagePath : "";

                await FillPermissions(advertisement, advertisementDto);

                return advertisementDto;
            }

            private async Task FillPermissions(Advertisement advertisement, AdvertisementDto advertisementDto)
            {
                var currentUser = _httpContextAccessor.HttpContext?.User;

                advertisementDto.Permissions = new List<string> {};
                if (currentUser == null)
                {
                    if(advertisement.State == AdvertisementState.Approved) advertisementDto.Permissions.Add(Constants.Read);
                }
                else
                {
                    var canView = await _authorizationService.AuthorizeAsync(currentUser,
                        advertisement, AdvertisementOperations.Read);
                    var canEdit = await _authorizationService.AuthorizeAsync(currentUser,
                        advertisement, AdvertisementOperations.Update);
                    var canChangeStatus = await _authorizationService.AuthorizeAsync(currentUser, advertisement,
                        AdvertisementOperations.ChangeStatus);
                    
                    if (canEdit.Succeeded)
                    {
                        advertisementDto.Permissions.Add(Constants.Update);
                        advertisementDto.Permissions.Add(Constants.Delete);
                    }

                    if (canView.Succeeded)
                    {
                        advertisementDto.Permissions.Add(Constants.Read); 
                    }

                    if (canChangeStatus.Succeeded)
                    {
                        advertisementDto.Permissions.Add(Constants.ChangeStatus);
                    }
                }
            }
        }
    }
}