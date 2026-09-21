using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Utility
{
    public class EmailClient
    {
        public String SendEmail(List<string> ToRecipients, List<string> CCRecipients, String Subject, String EmailBody)
        {
            EmailService _service = new EmailService();
            return _service.SendEmail(ToRecipients, CCRecipients, Subject, EmailBody);

        }
        public String SendEmailPublic(List<string> ToRecipients, List<string> CCRecipients, List<string> BCCRecipients, String Subject, String EmailBody)
        {
            EmailService _service = new EmailService();
            return _service.SendEmailPublic(ToRecipients, CCRecipients, BCCRecipients, Subject, EmailBody);

        }
    }
}