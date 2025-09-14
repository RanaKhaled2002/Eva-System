using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class CategoryTranslation : BaseClass
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
