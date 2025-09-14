using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class RegionHistoricalImage : BaseClass
    {
        public string ImageUrl { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}
