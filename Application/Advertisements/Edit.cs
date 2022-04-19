using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using ElasticSearch;
using ElasticSearch.Indexing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace Application.Advertisements
{
    public class Edit
    {
        public class Command : IRequest
        {
            public AdvertisementDto Advertisement { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IElasticSearchService _es;
            private readonly IAuthorizationService _authorizationService;

            public Handler(IHttpContextAccessor httpContextAccessor, DataContext context, IMapper mapper, IElasticSearchService es, IAuthorizationService authorizationService)
            {
                _httpContextAccessor = httpContextAccessor;
                _context = context;
                _mapper = mapper;
                _es = es;
                _authorizationService = authorizationService;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                Advertisement advertisement = await _context.Advertisements.FindAsync(request.Advertisement.Id);

                if (advertisement == null)
                {
                    throw new Exception("Advertisement does not exist");
                }

                if (request.Advertisement.Views < advertisement.Views)
                {
                    throw new Exception("Cant decrease advertisement views");
                }

                var currentUser = _httpContextAccessor.HttpContext?.User;
                if (currentUser == null)
                {
                    throw new Exception("Current user not found");
                }

                var isAuthorized = await _authorizationService.AuthorizeAsync(currentUser,
                    advertisement, AdvertisementOperations.Update); 
                
                if (!isAuthorized.Succeeded)
                {
                    throw new Exception($"Unauthorized to perform {Constants.Update} operation on this advertisement");
                }

                _mapper.Map(request.Advertisement, advertisement);
                
                await _context.SaveChangesAsync();
                await _es.Reindex(IndexDefinition.Advertisement, new List<Guid> {advertisement.Id});

                return Unit.Value;
            }
        }
    }
}