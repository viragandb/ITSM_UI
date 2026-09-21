using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ItemDoc
    {
        public long ItemDocId { get; set; }

        [Display(Name = "Type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Item")]
        [Required]
        public long ItemId { get; set; }

        [Display(Name = "File Name")]
        public string DocumentName { get; set; }

        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "URL")]
        public string FileUrl { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
