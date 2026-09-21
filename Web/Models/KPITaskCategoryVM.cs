using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class KPITaskCategoryVM
    {

        public string CategoryIndex { get; set; }
        public TaskCategory TaskCategory { get; set; }

        public ICollection<KPITaskVM> Tasks { get; set; }

        public double AllocatedScore { get; set; }
        public double Score { get; set; }
        public double ScoreValue { get; set; }
        public double WeightedValue { get; set; }
    }
}