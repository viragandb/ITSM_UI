using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [NotMapped]
    public class UserLogin
    {
        public long UserLoginId { get; set; }

        [Required]
        [Display(Name = "LAN Id")]
        public string UserName { get; set; }

        //[Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
