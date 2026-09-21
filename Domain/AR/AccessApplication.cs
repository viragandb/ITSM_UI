using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class AccessApplication
    {
        public long AccessApplicationId { get; set; }

        [Required]
        [Display(Name = "Access Privilege Category")]
        public long AccessPrivilegeCategoryId { get; set; }
        [ForeignKey("AccessPrivilegeCategoryId")]
        public virtual AccessPrivilegeCategory AccessPrivilegeCategory { get; set; }


        [Display(Name = "Application")]
        public long AssetTypeId { get; set; }
        public virtual AssetType System { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [NotMapped]
        public string ApplicationName { get; set; }


    }
}
