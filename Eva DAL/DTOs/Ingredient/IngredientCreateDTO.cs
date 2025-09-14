    using Eva_DAL.DTOs.Product;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

namespace Eva_DAL.DTOs.Ingredient
{
    public class IngredientCreateDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Accept only english letters")]
        public string TranslationKey { get; set; }

        [Required]
        public string Name { get; set; }

        public string? IngredientStory { get; set; }
        public string? Benefits { get; set; }
        public List<int>? ProductIds { get; set; }
        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? IngredientImages { get; set; }
    }

    public class IngredientDTO
    {
        public int Id { get; set; }
        public string TranslationKey { get; set; }
        public string Name { get; set; }
        public string? IngredientStory { get; set; }
        public string? Benefits { get; set; }
        public string? MainImage { get; set; }

        public List<IngredientImagesDTO>? IngredientImages { get; set; }

        public List<IngredientProductDTO>? IngredientProducts { get; set; }

        public List<IngredientTranlationDTO> IngredientTranlations { get; set; }
    }

    public class IngredientImagesDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
    }

    public class IngredientProductDTO
    {
        public string Name { get; set; }
    }

    public class IngredientTranlationDTO
    {
        public string Name { get; set; }
        public string? IngredientStory { get; set; }
        public string? Benefits { get; set; }
        public string LanguageCode { get; set; }
    }

    public class IngredientSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<IngredientTranlationDTO> IngredientTranslations { get; set; }
    }

    public class IngredientUpdateDTO
    {

        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Accept only english letters")]
        public string? TranslationKey { get; set; }

        public string? Name { get; set; }

        public string? IngredientStory { get; set; }
        public string? Benefits { get; set; }
        public List<int>? ProductIds { get; set; }
        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? IngredientImages { get; set; }
    }

    public class UpdateIngredientTranslationDto
    {
        public string? Name { get; set; }
        public string? IngredientStory { get; set; }
        public string? Benefits { get; set; }
    }


}

