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
                .ForMember(d => d.Category,
                    opt => 
                        opt.MapFrom(src => src.Category));
            CreateMap<AdvertisementDto, Advertisement>()
                .ForMember(d => d.CategoryId,
                opt => opt.MapFrom(src => src.Category.id))
                .ForMember(d => d.Category,
                opt => opt.Ignore());
            CreateMap<Category, CategoryDto>().PreserveReferences();
            CreateMap<CategoryDto, Category>();
            
            // Elastic Mappings
            CreateMap<AdvertisementSearchDocument, AdvertisementDto>();
            CreateMap<AdvertisementCategory, CategoryDto>();
            
        }
    }
}