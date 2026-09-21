using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Holiday
    {
        public long HolidayId { get; set; }

        [Required]
        [Display(Name = "Holiday Name")]
        public string HolidayName { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
