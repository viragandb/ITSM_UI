using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [NotMapped]
    public class AssetItemDTO
    {
        public string IMSCode { get; set; }
        public string ItemName { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public string Description { get; set; }
        public double ItemPrice { get; set; }

        public string SerialNo { get; set; }
        public string BarCode { get; set; }
        public string ModelNo { get; set; }
        public string EmpNo { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
    }
}
