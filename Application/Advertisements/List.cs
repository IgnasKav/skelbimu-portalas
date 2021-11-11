using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ElasticSearch;
using ElasticSearch.Indexing;
using ElasticSearch.SearchDocuments;
using MediatR;
using Persistence;

namespace Application.Advertisements
{
    public class List
    {
        public class Query : MediatR.IRequest<List<AdvertisementDto>>
        {
            public string SearchText { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<AdvertisementDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IElasticSearchService _es;
            
            public Handler(DataContext context, IMapper mapper, IElasticSearchService es)
            {
                _context = context;
                _mapper = mapper;
                _es = es;
            }
            
            public async Task<List<AdvertisementDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var elasticResult = await _es.Search<AdvertisementSearchDocument>(IndexDefinition.Advertisement, request.SearchText);
                var advertisements = elasticResult.Items;

                return _mapper.Map<List<AdvertisementSearchDocument>, List<AdvertisementDto>>(advertisements);
            }
        }
    }
}