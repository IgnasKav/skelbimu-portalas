using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using ElasticSearch;
using ElasticSearch.Indexing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace Application.Advertisements
{
    public class Create
    {
        public class Command : IRequest
        {
            public AdvertisementDto Advertisement { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly DataContext _context;
            private readonly IElasticSearchService _es;
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;

            public Handler(IHttpContextAccessor httpContextAccessor, DataContext context, IElasticSearchService es, IMapper mapper, UserManager<User> userManager)
            {
                _httpContextAccessor = httpContextAccessor;
                _context = context;
                _es = es;
                _mapper = mapper;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_httpContextAccessor.HttpContext?.User == null)
                {
                    throw new Exception("Couldn't find current user");
                }

                if (string.IsNullOrWhiteSpace(request.Advertisement.Title))
                {
                    throw new Exception("Advertisement must have title");
                }

                if (request.Advertisement.Views > 0)
                {
                    throw new Exception("Initial views must be 0");
                }

                var advertisementEntity = _mapper.Map<AdvertisementDto, Advertisement>(request.Advertisement);
                var currentUserId = new Guid(_userManager.GetUserId(_httpContextAccessor.HttpContext.User));
                advertisementEntity.OwnerId = currentUserId;
                advertisementEntity.State = AdvertisementState.New;

                var advertisement = _context.Advertisements.Add(advertisementEntity);

                await _context.SaveChangesAsync();
                await _es.Reindex(IndexDefinition.Advertisement, new List<Guid> {advertisement.Entity.Id});
                
                return Unit.Value;
            }
        }
    }
}