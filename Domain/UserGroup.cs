using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserGroup
    {
        public long UserGroupId { get; set; }

        [Required]
        public string EmpNo { get; set; }

        [Required]
        public long GroupId { get; set; }


        [DefaultValue("false")]
        [NotMapped]
        public bool IsSelected { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Group Group { get; set; }
        public virtual User User { get; set; }

    }
}
