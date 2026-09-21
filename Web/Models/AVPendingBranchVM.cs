using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AVPendingBranchVM
    {
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public virtual ICollection<AVPendingDeptVM> PendingDepartments{ get; set; }


    }
}