using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [NotMapped]
    public class LanIdDTO
    {
        public string LanId { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }

    }
}
