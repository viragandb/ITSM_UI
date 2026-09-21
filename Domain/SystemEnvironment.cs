using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class SystemEnvironment
    {
        public long SystemEnvironmentId { get; set; }

        [Display(Name = "Environment Type")]
        public SystemEnvironmentTypeEnum EnvironmentType { get; set; }

        [Display(Name = "System")]
        public long AssetTypeId { get; set; }
        public virtual AssetType System { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [NotMapped]
        public string SystemName { get; set; }

    }
}
