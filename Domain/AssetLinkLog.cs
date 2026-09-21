using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AssetLinkLog
    {
        public long AssetLinkLogId { get; set; }


        public long ParentAssetId { get; set; }
        public virtual Asset ParentAsset { get; set; }


        public long ChildAssetId { get; set; }
        public virtual Asset ChildAsset { get; set; }


        public AssetLinkActionEnum ActionType { get; set; }


        public string Description { get; set; }


        public string UpdatedBy { get; set; }
        
        
        public DateTime UpdatedDate { get; set; }


        public bool IsDeleted { get; set; }



    }
}
