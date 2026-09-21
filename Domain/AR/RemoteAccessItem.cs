using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class RemoteAccessItem
    {
        public long RemoteAccessItemId { get; set; }

        [Required]
        [Display(Name = "Application/System")]
        public long AssetTypeId { get; set; }
        public virtual AssetType AssetType { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
