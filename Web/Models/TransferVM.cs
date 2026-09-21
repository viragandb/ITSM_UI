using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TransferVM
    {


        [Display(Name = "Transaction Type")]
        public AssetTransactionTypeEnum TransactionType { get; set; }



        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Assign To ")]
        public string AssignTo { get; set; }
        [Display(Name = "Assign User Name ")]
        public string AssignUserName { get; set; }


        [Display(Name = "Location")]
        public long BranchIdFrom { get; set; }

        
        [Display(Name = "Department")]
        public long DepartmentIdFrom { get; set; }




        [Display(Name = "Transfer Location")]
        public long BranchId { get; set; }

        [Display(Name = "Transfer Department")]
        public long DepartmentId { get; set; }

        [Display(Name = "Vendor")]
        public long VendorId { get; set; }



        [Display(Name = "Assigned Type")]
        public AssignedTypeEnum AssignedType { get; set; }

        public List<AssetVM> Assets { get; set; }



    }
}