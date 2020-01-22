using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.Models
{
    public class EmailSettings
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
        public string APIKey { get; set; }
    }
}
