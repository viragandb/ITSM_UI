using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AVPendingDeptVM
    {
        public long DepartmentId { get; set; }
        public string Departmentname { get; set; }

        public string Status { get; set; }
        public string Date { get; set; }
        public long AssetVerificationId { get; set; }

    }
}