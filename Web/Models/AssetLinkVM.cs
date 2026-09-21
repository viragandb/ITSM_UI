using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AssetLinkVM
    {
        public long ParentAssetId { get; set; }

        public long ParentBranchIdFrom { get; set; }

        public long ParentDepartmentIdFrom { get; set; }

        public long ParentAssetCategoryId { get; set; }

        public long ParentAssetTypeId { get; set; } 

        public long ChildAssetTypeId { get; set; }

        public string ParentAssetNo { get; set; }

        public string ChildAssetNo { get; set; }

        public List<AssetVM> Assets { get; set; }

        public List<AssetVM> RemovedAssets { get; set; }
    }
}