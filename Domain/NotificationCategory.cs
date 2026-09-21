using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [Serializable]

    public class NotificationCategory
    {
        public long NotificationCategoryId { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Type")]
        public NotificationTypeEnum Type { get; set; }

        [Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "Controller")]
        public string Controller { get; set; }

        [Display(Name = "Icon")]
        public string Icon { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [NotMapped]
        public int NotificationCount { get; set; }
    }
}
