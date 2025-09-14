using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class IngredientTranslation : BaseClass
    {
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public string Name { get; set; }
        public string? Story { get; set; }
        public string? Benfits { get; set; }

    }
}
