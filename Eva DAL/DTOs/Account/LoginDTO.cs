using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_DAL.DTOs.Account
{
    public class LoginDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
