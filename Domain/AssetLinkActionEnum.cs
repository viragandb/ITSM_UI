using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum AssetLinkActionEnum : int
    {
        [Display(Name = "Linked")]
        Linked = 1,

        [Display(Name = "Unlinked")]
        Unlinked = 2
    }
}
