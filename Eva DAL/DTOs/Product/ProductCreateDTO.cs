using Eva_DAL.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eva_DAL.DTOs.Product
{

    public class ProductCreateDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Accept only english letters")]
        public string TranslationKey { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        public string? MediaDescription { get; set; }
        public string? Instruction { get; set; }

        [Required]
        public int categoryId { get; set; }

        public List<IFormFile>? ProductMedias { get; set; }

        public List<int>? IngredientIds { get; set; }
    }

    public class ProductDTO
    {
        public int Id { get; set; }
        public string TranslationKey { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Quantity { get; set; }
        public string? MediaDescription { get; set; }
        public string? Instruction { get; set; }

        public ProductCategoryDTO ProductCategory { get; set; }

        public List<ProductIngredientDTO> ProductIngredients { get; set; }

        public List<ProductTranlationDto> ProductTranslations { get; set; }

        public List<ProductMediaDTO> ImageUrls { get; set; }
    }

    public class ProductCategoryDTO
    {
        public int Id { get; set; }
        public string TransaltedKey { get; set; }
    }

    public class ProductIngredientDTO
    {
        public string Name { get; set; }
    }

    public class ProductTranlationDto
    {
        public string Name { get; set; }
        public string? MediaDescription { get; set; }
        public string? Instruction { get; set; }
        public string LanguageCode { get; set; }
    }

    public class ProductSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductTranlationDto> ProductTranslations { get; set; }
    }

    public class ProductUpdateDTO
    {
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Accept only english letters")]
        public string? TranslationKey { get; set; }

        public string? Name { get; set; }

        public string? Size { get; set; }

        public decimal? Quantity { get; set; }

        public string? MediaDescription { get; set; }
        public string? Instruction { get; set; }

        public int? categoryId { get; set; }

        public List<IFormFile>? ProductMedias { get; set; }

        public List<int>? IngredientIds { get; set; }
    }

    public class ProductMediaDTO
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }

    public class UpdateProductTranslationDto
    {
        public string? Name { get; set; }
        public string? MediaDescription { get; set; }
        public string? Instruction { get; set; }
    }
}
