using AutoMapper;
using Eva_DAL.DTOs.Ingredient;
using Eva_DAL.DTOs.Region;
using Eva_DAL.Models;

namespace Eva_API.Mapping
{
    public class RegionProfile : Profile
    {
        public RegionProfile(IConfiguration configuration)
        {
            CreateMap<RegionCreateDTO, Region>()
               .ForMember(dest => dest.Ingredients, opt => opt.Ignore())
               .ForMember(dest => dest.Translations, opt => opt.Ignore())
               .ForMember(dest => dest.RegionCulturalImages, opt => opt.Ignore())
               .ForMember(dest => dest.RegionHistoricalImages, opt => opt.Ignore());

            CreateMap<Region, RegionDTO>()
                .ForMember(dest => dest.MainImage, opt =>
                    opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.MainImageUrl)
                            ? null
                            : $"{configuration["BaseUrl"]}/{src.MainImageUrl}"
                    ))
                .ForMember(dest => dest.RegionCulturalImages, opt => opt.MapFrom(src => src.RegionCulturalImages))
                .ForMember(dest => dest.RegionHistoricalImages, opt => opt.MapFrom(src => src.RegionHistoricalImages))
                .ForMember(dest => dest.RegionIngredients, opt => opt.MapFrom(src => src.Ingredients))
                .ForMember(dest => dest.RegionTranslations, opt => opt.MapFrom(src => src.Translations))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<RegionCulturalImage, RegionCulturalImagesDTO>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.ImageUrl)
                            ? null
                            : $"{configuration["BaseUrl"]}/{src.ImageUrl}"
                    ));

            CreateMap<RegionHistoricalImage, RegionHistoricalImagesDTO>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.ImageUrl)
                            ? null
                            : $"{configuration["BaseUrl"]}/{src.ImageUrl}"
                    ));

            CreateMap<Ingredient, RegionIngredientDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<RegionTranslation, RegionTranslationDTO>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.LangShortCode));
        }
    }
}
