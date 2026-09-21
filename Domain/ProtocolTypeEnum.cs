using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
     public enum ProtocolTypeEnum : int
    {

        //[Display(Name = "TCP/IP")]
        //TCPIP = 1,

        [Display(Name = "HTTP")]
        HTTP = 2,

        [Display(Name = "HTTPS")]
        HTTPS = 3,

        [Display(Name = "SMTP")]
        SMTP = 4,

        [Display(Name = "FTP")]
        FTP = 5,

        [Display(Name = "SFTP")]
        SFTP = 6,
    }
}
