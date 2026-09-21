using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class WorkTaskVM
    {

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Task User")]
        public string TaskUserId { get; set; }

        [Required]
        [Display(Name = "Task Date")]
        public DateTime TaskDate { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public String StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public String EndTime { get; set; }


        [Display(Name = "Spent Hrs.")]
        public double SpentTime { get; set; }

        [Display(Name = "Type")]
        public TaskTypeEnum Type { get; set; }

        [Display(Name = "Status")]
        public TaskStatusEnum Status { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }


    }
}