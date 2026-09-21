using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum ProblemStatusEnum: int
    {
        [Display(Name = "Pending")]
        Pending = 1,

        [Display(Name = "Analysed")]
        Analysed = 2,

        [Display(Name = "Closed")]
        Closed = 3

        

    }
}
