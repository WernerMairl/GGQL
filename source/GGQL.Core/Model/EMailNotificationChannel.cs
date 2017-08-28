using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace GGQL.Core
{
    public class EMailNotificationChannel : SmtpClientNotificationChannel
    {

        public EMailNotificationChannel()
        {
            //this.UseSsl = true;
        }

        public string SmtpHost { get; set; }

        protected override SmtpClient CreateSmtpClient()
        {
            SmtpClient cl = new SmtpClient();
            cl.EnableSsl = false; //nt configured
            cl.Host = this.SmtpHost;
            return cl;
        }

    }

}


