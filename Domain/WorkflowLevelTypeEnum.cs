using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum WorkflowLevelTypeEnum : int
    {
        //[Display(Name = "Inputter")]
        //Team = 1,

        //[Display(Name = "Level 01")]
        //Level01 = 5,

        //[Display(Name = "Team Head")]
        //TeamHead = 10,

        //[Display(Name = "Line AVP")]
        //LineAVP = 20,

        //[Display(Name = "IT AVP")]
        //ITAVP = 21,

        //[Display(Name = "IT VP")]
        //ITVP = 22,


        [Display(Name = "N/A")]
        NA = 0,


        //[Display(Name = "Initiator")]
        //Initiator = 1,

        [Display(Name = "Inputter")]
        Inputter = 2,

        [Display(Name = "Checker")]
        Checker = 3,

        [Display(Name = "Authorizer")]
        Authorizer = 4,

        [Display(Name = "Implementer")]
        Implementer = 5,

       
    }
}
