using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class Ingredient : BaseClass
    {
        public string Name { get; set; }
        public string? MainIngredientImageUrl { get; set; }
        public string? Story { get; set; }
        public string? Benefits { get; set; }

        public int? RegionId { get; set; }
        public Region? Region { get; set; }

        public string TranslationKey { get; set; }

        public ICollection<ProductIngredient>? ProductIngredients { get; set; }
        public ICollection<IngredientTranslation>? Translations { get; set; }
        public ICollection<IngredientImage>? IngredientImages { get; set; }
    }
}
