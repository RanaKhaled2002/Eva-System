using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class Language : BaseClass
    {
        public string Lang { get; set; }
        public string LangShortCode { get; set; }
        public string Direction { get; set; }

        public ICollection<ProductTranslation> ProductTranslations { get; set; }
        public ICollection<CategoryTranslation> CategoryTranslations { get; set; }
        public ICollection<IngredientTranslation> IngredientTranslations { get; set; }
        public ICollection<RegionTranslation> RegionTranslations { get; set; }
    }
}
