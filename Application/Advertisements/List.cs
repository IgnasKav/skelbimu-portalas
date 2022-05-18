using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using ElasticSearch;
using ElasticSearch.SearchDocuments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Nest;

namespace Application.Advertisements
{
    public class List
    {
        public class Query : MediatR.IRequest<List<AdvertisementDto>>
        {
            public ElasticSearchRequest ElasticSearchRequest { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<AdvertisementDto>>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMapper _mapper;
            private readonly IElasticSearchService _es;
            private readonly UserManager<User> _userManager;

            public Handler(IHttpContextAccessor httpContextAccessor, IMapper mapper, IElasticSearchService es, UserManager<User> userManager)
            {
                _httpContextAccessor = httpContextAccessor;
                _mapper = mapper;
                _es = es;
                _userManager = userManager;
            }
            
            public async Task<List<AdvertisementDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

                if (currentUser == null) throw new Exception("Current user not found");

                var userRoles = (await _userManager.GetRolesAsync(currentUser)).ToList();

                var filters = new List<QueryContainer>();

                if (!userRoles.Contains("Admin") && !userRoles.Contains("Support") && string.IsNullOrWhiteSpace(request.ElasticSearchRequest.UserId))
                {
                    var advertisementStatusFilter = new TermQuery
                    {
                        Field = Infer.Field<AdvertisementSearchDocument>(f => f.State),
                        Value = AdvertisementState.Approved
                    };
                    
                    filters.Add(advertisementStatusFilter);
                }

                if (!string.IsNullOrWhiteSpace(request.ElasticSearchRequest.UserId))
                {
                    var userIdFilter = new MatchQuery()
                    {
                        Field = Infer.Field<AdvertisementSearchDocument>(f => f.OwnerId),
                        Query = request.ElasticSearchRequest.UserId
                    };
                    
                    filters.Add(userIdFilter);
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

                var elasticResult = await _es.Search<AdvertisementSearchDocument>(request.ElasticSearchRequest, filters, categoriesFilter);
                var advertisements = elasticResult.Items;

                return _mapper.Map<List<AdvertisementSearchDocument>, List<AdvertisementDto>>(advertisements);
            }
        }
    }
}