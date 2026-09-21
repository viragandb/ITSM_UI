using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CallLog
    {
        public long CallLogId { get; set; }

        [Display(Name = "User Id")]
        public string EmpNo { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Total Online Hrs")]
        public double OnlineHrs { get; set; }
                       

        [Display(Name = "No Of Missed Calls")]
        public double MissedCalls { get; set; }

        [Display(Name = "No Of Received Calls")]
        public double ReceivedCalls { get; set; }

        [Display(Name = "Total Abundant Calls")]
        public double TotalAbundantCalls { get; set; }

        [Display(Name = "Total Received Calls")]
        public double TotalReceivedCalls { get; set; }


        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }


        [Display(Name = "User")]
        public User User { get; set; }
    }
}
