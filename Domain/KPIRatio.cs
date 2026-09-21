using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class KPIRatio
    {

        public long KPIRatioId { get; set; }

        [Display(Name = "User Id")]
        public string EmpNo { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Total Online Hrs")]
        public double OnlineHrs { get; set; }

        [Display(Name = "Total Ticket Hrs")]
        public double TicketHrs { get; set; }


        [Display(Name = "Total Task Hrs")]
        public double TaskHrs { get; set; }

        [Display(Name = "Total Working Hrs")]
        public double WorkingHrs { get; set; }

        [Display(Name = "No Of Missed Calls")]
        public double MissedCalls { get; set; }

        [Display(Name = "No Of Received Calls")]
        public double ReceivedCalls { get; set; }

        [Display(Name = "Total Abundant Calls")]
        public double TotalAbundantCalls { get; set; }

        [Display(Name = "Total Received Calls")]
        public double TotalReceivedCalls { get; set; }

        [Display(Name = "Total Created Tickets")]
        public double TotalCreatedTickets { get; set; }

        [Display(Name = "SLS Violated Tickets")]
        public double SLAViolatedTickets { get; set; }

        [Display(Name = "Total Updated Tickets")]
        public double TotalUpdatedTickets { get; set; }

        [Display(Name = "First Level Resolve Tickets")]
        public double ResolveFLTickets { get; set; }

        [Display(Name = "Total Assigned Tickets")]
        public double TotalAssignedTickets { get; set; }

        [Display(Name = "Total Work Time Ratio")]
        public double RatioWorkingTime { get; set; }

        [Display(Name = "Answered Call Ratio")]
        public double RatioAnswerdCall { get; set; }

        [Display(Name = "Ticket Entering Ratio")]
        public double RatioTicketEntering { get; set; }

        [Display(Name = "Abundant Call Ratio ")]
        public double RatioAbundantCall { get; set; }

        [Display(Name = "SLA violation Ratio")]
        public double RatioSLA { get; set; }

        [Display(Name = "First Level Resolution Ratio")]
        public double RatioFirstLResolve { get; set; }

        [Display(Name = "Final Value ")]
        public double FinalValue { get; set; }

        [Display(Name = "User")]
        public User User { get; set; }


        [NotMapped]
        public ICollection<KPIRatio> History { get; set; }


        
    }
}
