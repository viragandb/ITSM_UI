using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ItemAsset
    {
        public long ItemAssetId { get; set; }

        [Display(Name = "Type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Item")]
        [Required]
        public long ItemId { get; set; }

        [Display(Name = "Ticket")]
        public long? TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [Required]
        [Display(Name = "Asset")]
        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

       
    }
}
