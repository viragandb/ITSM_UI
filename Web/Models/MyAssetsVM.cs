using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class MyAssetsVM
    {
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<AssetTransferRequest> AssetTransferRequests { get; set; }

    }
}