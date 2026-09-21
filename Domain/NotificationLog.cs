using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [Serializable]
    public class NotificationLog
    {
        public long NotificationLogId { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Notification Type")]
        public NotificationTypeEnum Type { get; set; }

        public long EventId { get; set; }

        public DateTime CreatedDate { get; set; }

        public long NotificationCategoryId { get; set; }
        public virtual NotificationCategory NotificationCategory { get; set; }


    }
}
