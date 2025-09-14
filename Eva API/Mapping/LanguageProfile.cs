using AutoMapper;
using Eva_DAL.DTOs.Language;
using Eva_DAL.Models;

namespace Eva_API.Mapping
{
    public class LanguageProfile : Profile
    {
        public LanguageProfile()
        {
            CreateMap<Language, LanguageDTO>().ReverseMap();
            CreateMap<Language, UpdateLanguageDTO>().ReverseMap();
            CreateMap<Language, ShowLanguageDTO>().ReverseMap();
        }
    }
}
