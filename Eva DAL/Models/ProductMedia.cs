using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class ProductMedia : BaseClass
    {
        public string ImageUrl { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
