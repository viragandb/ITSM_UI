using Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AssetVerificationVM
    {
        [Display(Name = "Location")]
        public long BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        [Display(Name = "Department")]
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Display(Name = "Planned Verification Date")]
        public String VerificationDate { get; set; }

        [Display(Name = "Asset Category")]
        public AssetCategoryEnum AssetCategory { get; set; }


        public List<Asset> Assets { get; set; }
    }
}