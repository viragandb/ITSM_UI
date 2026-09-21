using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ItemLog
    {
        public long ItemLogId { get; set; }

        [Display(Name = "Type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Item")]
        [Required]
        public long ItemId { get; set; }


        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }

    }
}
