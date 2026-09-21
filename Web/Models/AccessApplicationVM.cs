using Domain;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AccessApplicationVM
    {
        public long AccessApplicationId { get; set; }

        [Display(Name = "Asset Category")]
        public AssetCategoryEnum AssetCategory { get; set; }

        [Display(Name = "System")]
        public long AssetTypeId { get; set; }
        public virtual AssetType System { get; set; }

        [Display(Name = "Access Privilege Category")]
        public long AccessPrivilegeCategoryId { get; set; }
        public virtual AccessPrivilegeCategory AccessPrivilegeCategory { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string ApplicationName { get; set; }
    }
}