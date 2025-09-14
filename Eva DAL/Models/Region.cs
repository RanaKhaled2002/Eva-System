using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class Region : BaseClass
    {
        public string Name { get; set; }
        public string? MainImageUrl { get; set; }
        public string? HistoricalContent { get; set; }
        public string? CulturalContent { set; get; }

        public string TranslationKey { get; set; }


        public ICollection<Ingredient>? Ingredients { get; set; }
        public ICollection<RegionTranslation>? Translations { get; set; }
        public ICollection<RegionCulturalImage>? RegionCulturalImages { get; set; }
        public ICollection<RegionHistoricalImage>? RegionHistoricalImages { get; set; }

    }
}
