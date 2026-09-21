using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Domain.DR;

namespace Web.Models
{
    public class DisposalVM
    {
        public long DisposalRequestId { get; set; }


        [Display(Name = "Disposal Request Title")]
        public string Title { get; set; }


        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Location")]
        public long BranchIdFrom { get; set; }


        [Display(Name = "Department")]
        public long DepartmentIdFrom { get; set; }


        public List<AssetVM> Assets { get; set; }

        public List<AssetVM> RemovedAssets { get; set; }

        public ICollection<DisposalRequest> AllDisposalRequests { get; set; }

    }
}