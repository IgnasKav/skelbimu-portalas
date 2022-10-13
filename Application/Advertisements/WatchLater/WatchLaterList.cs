using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using ElasticSearch;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using AutoMapper;
using ElasticSearch.SearchDocuments;
using Microsoft.EntityFrameworkCore;
using Nest;
using Persistence;

namespace Application.Advertisements.WatchLater;

public class WatchLaterList
{
    public class Query : MediatR.IRequest<List<AdvertisementDto>>
    {
        public ElasticSearchRequest ElasticSearchRequest { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<AdvertisementDto>>
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IElasticSearchService _es;
    
        public Handler(DataContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IMapper mapper, IElasticSearchService es)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _es = es;
            _userManager = userManager;
        }
        
        public async Task<List<AdvertisementDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUserClaim = _httpContextAccessor.HttpContext?.User;
            if (currentUserClaim == null)
            {
                throw new Exception("Current user not found");
            }
        
            var filters = new List<QueryContainer>();
        
            var currentUserId = new Guid(_userManager.GetUserId(currentUserClaim));
            var watchLaterList = await _context.WatchLater
                .Where(w => w.UserId == currentUserId)
                .ToListAsync(cancellationToken);
            
            if (watchLaterList.Count > 0)
            {
                var advertisementIds = watchLaterList.Select(w =>  (Id)w.AdvertisementId).ToList();
                var watchLaterFilter = new TermsQuery()
                {
                    Field = Infer.Field<AdvertisementSearchDocument>(f => f.Id),
                    Terms =  advertisementIds
                };
                                        
                filters.Add(watchLaterFilter);
            }
        
            var categoriesFilter = new List<QueryContainer>();
        
            if (request.ElasticSearchRequest.CategoryFilters != null)
            {
                foreach (var categoryFilter in request.ElasticSearchRequest.CategoryFilters)
                {
                    var elasticFilter = new MatchQuery
                    {
                        Field = Infer.Field<AdvertisementSearchDocument>(f => f.CategoryFilter),
                        Query = categoryFilter.CategoryFilter,
                        Operator = Operator.And
                    };
        
                    categoriesFilter.Add(elasticFilter);
                }
            }

            var sortQuery = new List<ISort>
            {
                new FieldSort
                {
                    Field = Infer.Field<AdvertisementSearchDocument>(f => f.Date),
                    Order = SortOrder.Descending
                }
            };
        
            var elasticResult = await _es.Search<AdvertisementSearchDocument>(request.ElasticSearchRequest, sortQuery, filters, categoriesFilter);
            var advertisements = elasticResult.Items;
            return _mapper.Map<List<AdvertisementSearchDocument>, List<AdvertisementDto>>(advertisements);
        }
    }
}