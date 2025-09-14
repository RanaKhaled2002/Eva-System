using AutoMapper;
using Eva_DAL.DTOs.Product;
using Eva_DAL.Models;

namespace Eva_API.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile(IConfiguration configuration)
        {
            // Map from CreateDTO → Product
            CreateMap<ProductCreateDTO, Product>()
                .ForMember(dest => dest.ProductIngredients, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.ProductMedias, opt => opt.Ignore());

            // Map from UpdateDTO → Product
            CreateMap<ProductUpdateDTO, Product>()
                .ForMember(dest => dest.ProductIngredients, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.ProductMedias, opt => opt.Ignore());

            // Map from Product → ProductDTO
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.ProductIngredients, opt => opt.MapFrom(src => src.ProductIngredients))
                .ForMember(dest => dest.ProductTranslations, opt => opt.MapFrom(src => src.Translations))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ProductMedias)
                );

            // Category → DTO
            CreateMap<Category, ProductCategoryDTO>()
                .ForMember(dest => dest.TransaltedKey, opt => opt.MapFrom(src => src.TranslationKey));

            // ProductIngredient → DTO
            CreateMap<ProductIngredient, ProductIngredientDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Ingredient.Name));

            // ProductTranslation → DTO
            CreateMap<ProductTranslation, ProductTranlationDto>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.LangShortCode));

            CreateMap<ProductMedia, ProductMediaDTO>()
                .ForMember(dest => dest.Url, opt =>
                    opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.ImageUrl)
                            ? null
                            : $"{configuration["BaseUrl"]}/{src.ImageUrl}"
                    ));
        }
    }
}
