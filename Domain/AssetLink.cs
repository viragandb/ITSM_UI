using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DR;

namespace Domain
{
    public class AssetLink
    {
        public long AssetLinkId { get; set; }


        public long ParentAssetId { get; set; }
        public virtual Asset ParentAsset { get; set; }


        public long ChildAssetId { get; set; }
        public virtual Asset ChildAsset { get; set; }

        
        public string UpdatedBy { get; set; }


        public DateTime UpdatedDate { get; set; }


        public bool IsDeleted { get; set; }
    }
}
