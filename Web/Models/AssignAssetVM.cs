using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AssignAssetVM
    {


        //public virtual Category Category { get; set; }

        [Display(Name = "Asset Category")]
        public AssetCategoryEnum AssetCategory { get; set; }

        public long ItemId { get; set; }
        public Ticket Ticket { get; set; }

        public virtual ICollection<ItemAsset> AssignedItems { get; set; }

        public virtual ICollection<Asset> LocationItems { get; set; }
        //public virtual ICollection<AssetItemDTO> LocationDCItems { get; set; }
        public virtual ICollection<Asset> UserItems { get; set; }

    }

}