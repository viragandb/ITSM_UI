using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.RA;

namespace Domain
{
    public class Team
    {
        public long TeamId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string TeamName { get; set; }

        [Display(Name = "Team Email")]
        public string TeamEmail { get; set; }


        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public virtual ICollection<UserTeam> UserTeams { get; set; }

        public virtual ICollection<RemoteAccessRequestUpdate> ClarificationRequestedByTeamUpdates { get; set; }

        public virtual ICollection<RemoteAccessRequestUpdate> ClarificationAssignedToTeamUpdates { get; set; }

    }
}
