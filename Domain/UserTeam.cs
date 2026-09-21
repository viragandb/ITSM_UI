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
    public class UserTeam
    {
        public long UserTeamId { get; set; }

        [Required]
        [Display(Name = "Team Name")]
        public long TeamId { get; set; }

        [Required]
        [Display(Name = "User")]
        public string EmpNo { get; set; }

        [DefaultValue("false")]
        [NotMapped]
        public bool IsSelected { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Team Team { get; set; }
        public virtual User User { get; set; }
    }
}
