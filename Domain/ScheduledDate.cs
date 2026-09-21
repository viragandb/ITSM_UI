using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ScheduledDate
    {
        public long ScheduledDateId { get; set; }


        [Display(Name = "Task")]
        public long ScheduledTaskId { get; set; }

        [Display(Name = "Day/Date")]
        public string TaskDay { get; set; }

        [Display(Name = "Day No")]
        public int TaskDayNo { get; set; }

        [Display(Name = "Date")]
        public DateTime TaskDate { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ScheduledTask ScheduledTask { get; set; }

    }
}
