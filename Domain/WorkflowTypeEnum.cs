using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum WorkflowTypeEnum : int
    {
        [Display(Name = "Access Request")]
        AccessRequest = 1,

    }
}
