using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.Models
{
    public class RevokedToken : BaseClass
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
