using Domain;
using Domain.IM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class NewIncidentRequestVM
    {
        public long IncidentRequestId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }


        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Level 01 Support")]
        public long TeamId { get; set; }
        public virtual Team Level01Team { get; set; }

        [Required]
        [Display(Name = "Occurred Date")]
        public String OccurredDate { get; set; }

        [Required]
        [Display(Name = "Occurred Time")]
        public String OccurredTime { get; set; }


        [Required]
        [Display(Name = "Impact")]
        public IncidentImpactEnum Impact { get; set; }

        [Required]
        [Display(Name = "Business Impact")]
        public String BusinessImpact { get; set; }

        public List<TicketMinVM> Tickets { get; set; }




    }
}