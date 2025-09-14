using AutoMapper;
using Eva_DAL.DTOs.Ingredient;
using Eva_DAL.DTOs.Product;
using Eva_DAL.Models;

namespace Eva_API.Mapping
{
    public class IngredientProfile : Profile
    {
        public IngredientProfile(IConfiguration configuration)
        {
            CreateMap<IngredientCreateDTO, Ingredient>()
                .ForMember(dest => dest.ProductIngredients, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.IngredientImages, opt => opt.Ignore())
                .ForMember(dest => dest.MainIngredientImageUrl, opt => opt.Ignore());

            CreateMap<IngredientUpdateDTO, Ingredient>()
                .ForMember(dest => dest.ProductIngredients, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.IngredientImages, opt => opt.Ignore())
                .ForMember(dest => dest.MainIngredientImageUrl, opt => opt.Ignore());

            CreateMap<Ingredient, IngredientDTO>()
                .ForMember(dest => dest.MainImage, opt =>
                    opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.MainIngredientImageUrl)
                            ? null
                            : $"{configuration["BaseUrl"]}/{src.MainIngredientImageUrl}"
                    ))
                .ForMember(dest=>dest.IngredientStory,opt=>opt.MapFrom(src=>src.Story))
                .ForMember(dest=>dest.Benefits,opt=>opt.MapFrom(src=>src.Benefits))
                .ForMember(dest => dest.IngredientImages, opt => opt.MapFrom(src => src.IngredientImages))
                .ForMember(dest => dest.IngredientProducts, opt => opt.MapFrom(src => src.ProductIngredients))
                .ForMember(dest => dest.IngredientTranlations, opt => opt.MapFrom(src => src.Translations));


            CreateMap<IngredientImage, IngredientImagesDTO>()
                .ForMember(dest => dest.ImageUrl, opt =>
                    opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.ImageUrl)
                            ? null
                            : $"{configuration["BaseUrl"]}/{src.ImageUrl}"
                    ));

            // ProductIngredient → DTO
            CreateMap<ProductIngredient, IngredientProductDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name));

            // ProductTranslation → DTO
            CreateMap<IngredientTranslation, IngredientTranlationDTO>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.LangShortCode))
                .ForMember(dest => dest.IngredientStory, opt => opt.MapFrom(src => src.Story))
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src => src.Benfits));
        }
    }
}
