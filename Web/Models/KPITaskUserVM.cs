using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class KPITaskUserVM
    {

        public User User { get; set; }

        public ICollection<KPITaskCategoryVM> TaskCategories { get; set; }

        public double KPIValueWeightage { get; set; }
        public double KPIValue { get; set; }
    }
}