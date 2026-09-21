using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    public class DowntimeAlertEmail
    {
        public long DowntimeAlertEmailId { get; set; }

        public long ChangeImplementDataId { get; set; }
        public virtual ChangeImplementData ChangeImplementData { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }
}
