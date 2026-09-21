using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum TaskTypeEnum:int
    {
        [Display(Name = "Task")]
        Task = 1
    }
}
