using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class Product : BaseClass
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Quantity { get; set; }
        public string? MediaDescription { get; set; }
        public string? Instruction { get; set; }

        public int? categoryId { get; set; }
        public Category? Category { get; set; }

        public string TranslationKey { get; set; }

        public ICollection<ProductIngredient>? ProductIngredients { get; set; }
        public ICollection<ProductTranslation>? Translations { get; set; }
        public ICollection<ProductMedia>? ProductMedias { get; set; }
    }
}
