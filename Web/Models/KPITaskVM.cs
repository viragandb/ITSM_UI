using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class KPITaskVM
    {

        public long TaskId { get; set; }
        public string TaskName { get; set; }
        public double AllocatedScore { get; set; }
        public double Score { get; set; }
        public double NoOfTasks { get; set; }
        public double ScoreValue { get; set; }

        public ScheduledTask Task { get; set; }

    }
}