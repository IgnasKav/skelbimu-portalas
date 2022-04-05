using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

            public Handler(IHttpContextAccessor httpContextAccessor, DataContext context, IMapper mapper, IAuthorizationService authorizationService)
            {
                _httpContextAccessor = httpContextAccessor;
                _context = context;
                _mapper = mapper;
                _authorizationService = authorizationService;
            }

            public async Task<AdvertisementDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var advertisement = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                await _context.Entry(advertisement).Reference(x => x.Category).LoadAsync(cancellationToken);
                
                var advertisementDto = _mapper.Map<Advertisement, AdvertisementDto>(advertisement);
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