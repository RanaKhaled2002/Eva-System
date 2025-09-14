using Eva_DAL.DTOs.Ingredient;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.DTOs.Region
{
    public class RegionCreateDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Accept only english letters")]
        public string TranslationKey { get; set; }

        [Required]
        public string Name { get; set; }

        public string? HistoricalContent { get; set; }
        public string? CulturalContent { get; set; }
        public List<int>? IngredientIds { get; set; }
        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? RegionCulturalImages { get; set; }
        public List<IFormFile>? RegionHistoricalImages { get; set; }
    }

    public class RegionDTO()
    {
        public int Id { get; set; }
        public string TranslationKey { get; set; }
        public string Name { get; set; }
        public string? HistoricalContent { get; set; }
        public string? CulturalContent { get; set; }
        public string MainImage { get; set; }
        public List<RegionIngredientDTO> RegionIngredients {get;set;}
        public List<RegionCulturalImagesDTO> RegionCulturalImages { get; set; }
        public List<RegionHistoricalImagesDTO> RegionHistoricalImages { get; set; }
        public List<RegionTranslationDTO> RegionTranslations { get; set; }
    }

    public class RegionIngredientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RegionCulturalImagesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RegionHistoricalImagesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RegionTranslationDTO
    {
        public string Name { get; set; }
        public string? HistoricalContent { get; set; }
        public string? CulturalContent { get; set; }
        public string LanguageCode { get; set; }
    }

    public class UpdateRegionTranslationDto
    {
        public string? Name { get; set; }
        public string? HistoricalContent { get; set; }
        public string? CulturalContent { get; set; }
    }

    public class RegionSearchDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RegionTranslationDTO> RegionTranslations { get; set; }
    }

    public class RegionUpdateDTO
    {
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Accept only english letters")]
        public string? TranslationKey { get; set; }

        public string? Name { get; set; }

        public string? HistoricalContent { get; set; }
        public string? CulturalContent { get; set; }
        public List<int>? IngredientIds { get; set; }
        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? RegionCulturalImages { get; set; }
        public List<IFormFile>? RegionHistoricalImages { get; set; }
    }
}
