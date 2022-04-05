using Application.Advertisements;
using Application.Categories;
using AutoMapper;
using Domain;
using ElasticSearch.SearchDocuments;
using ElasticSearch.SearchDocuments.NestedTypes;

namespace Application.Core
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Advertisement, Advertisement>();
            CreateMap<Advertisement, AdvertisementDto>()
                .ForMember(d => d.Category, opt => opt.MapFrom(src => src.Category));
            CreateMap<AdvertisementSearchDocument, AdvertisementDto>();
            CreateMap<AdvertisementCategory, CategoryDto>();
            CreateMap<Category, CategoryDto>().PreserveReferences();
        }
    }
}