using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class SystemEnvironmentVM
    {
        public long SystemEnvironmentId { get; set; }

        [Display(Name = "Environment Type")]
        public SystemEnvironmentTypeEnum EnvironmentType { get; set; }

        [Display(Name = "Asset Category")]
        public AssetCategoryEnum AssetCategory { get; set; }

        [Display(Name = "System")]
        public long AssetTypeId { get; set; }
        public virtual AssetType System { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string SystemName { get; set; }
    }
}