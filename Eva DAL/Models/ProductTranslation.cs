using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class ProductTranslation : BaseClass
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public string Name { get; set; }
        public string? MediaDescription { get; set; }
        public string? Instruction { get; set; }

    }
}
