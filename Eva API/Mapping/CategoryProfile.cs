using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Eva_DAL.DTOs.Category;
using Eva_DAL.DTOs.Language;
using Eva_DAL.Models;

namespace Eva_API.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap <Eva_DAL.Models.Category, CategoryDTO>().ReverseMap();

            CreateMap<Eva_DAL.Models.Category, CategoryWithTranslationsDTO>();
            CreateMap<Eva_DAL.Models.Category, CategoryUpdateDTO>();
            CreateMap<CategoryTranslation, CategoryTranslationDTO>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.LangShortCode));
        }
    }
}
