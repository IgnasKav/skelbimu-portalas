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
        //sutvarkyt mappingus (pazet docs)
        public MappingProfiles()
        {
            CreateMap<Advertisement, Advertisement>();
            CreateMap<Advertisement, AdvertisementDto>()
                .ForMember(d => d.Category, opt => opt.MapFrom(src => src.Category));
            CreateMap<AdvertisementSearchDocument, AdvertisementDto>();
            CreateMap<AdvertisementCategory, CategoryDto>();
            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.parentId, o => o.MapFrom(s => s.ParentId));
        }
    }
}