using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DayOfWeekVM
    {

        [Display(Name = "Day")]
        public string Day { get; set; }
        public int DayNo { get; set; }

        [DefaultValue("false")]
        public bool IsSelected { get; set; }

    }
}