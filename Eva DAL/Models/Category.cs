using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class Category : BaseClass
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public string TranslationKey { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<CategoryTranslation> Translations { get; set; }
    }
}
