using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Accessory
    {
        public long AccessoryId { get; set; }

        //[Display(Name = "Asset")]
        //public long AssetId { get; set; }
        //public virtual Asset Asset { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public virtual User AssignedToUser { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
