using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.DTOs.Category
{
    public class CategoryDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Accept only english letters")]
        public string TranslationKey { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

    }

    public class CategoryTranslationDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string LanguageCode { get; set; }
    }

    public class CategoryWithTranslationsDTO
    {
        public int Id { get; set; }
        public string TranslationKey { get; set; }
        public string? Description { get; set; }

        public List<CategoryTranslationDTO> Translations { get; set; }
    }

    public class CategoryUpdateDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Accept only english letters")]
        public string TranslationKey { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

    }
}
