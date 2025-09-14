using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class RegionTranslation : BaseClass
    {
        public int RegionId { get; set; }
        public Region Region { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public string Name { get; set; }
        public string? HistoricalContent { get; set; }
        public string? CulturalContent { get; set; }
    }
}
