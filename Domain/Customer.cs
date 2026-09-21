using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [NotMapped]
    public class Customer
    {

        //public int CID { get; set; }

        [Display(Name = "Customer Id")]
        public string CustomerCode { get; set; } //cid

        [Display(Name = "NIC/PP/Business Reg No")]
        public string IdentificationNo { get; set; } //identification_num
     

        [Display(Name = "Account Hold Branch")]
        public string AccountHoldBranch { get; set; } 

        [Display(Name = "Account Officer")]
        public string AccountOfficer { get; set; } //account_officer

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } //given_name

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; } //mobile_num

        [Display(Name = "Date Of Birth")]
        public string DateOfBirthSrt { get; set; } //date_of_birth

        [Display(Name = "Email")]
        public string Email { get; set; } //email
        public string CardNo { get; set; } 

    }
}
