using GGQL.Core.Internal;
using GGQL.Model;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace GGQL.Core
{
    public class SmtpClientNotificationChannel : NotificationChannel
    {
        public static MailAddress Convert(NotificationAddress address)
        {
            if (string.IsNullOrEmpty(address.DisplayName))
            {
                return new MailAddress(address.Identifier);
            }
            return new MailAddress(address.Identifier, address.DisplayName);
        }

        private MailMessage CreateMailMessage(Notification notification)
        {
            MailMessage message = new MailMessage
            {
                Subject = notification.Subject,
                Body = notification.MessageBody,
                IsBodyHtml = notification.IsHtml,
                From = Convert(notification.From)
            };

            foreach (NotificationAddress na in notification.To)
            {
                message.To.Add(Convert(na));
            }
            message.Body = notification.MessageBody;
            message.Subject = notification.Subject;

            return message;
        }

        internal Func<SmtpClient> CreateSmtpClientDelegate { get; set; }

        protected virtual SmtpClient CreateSmtpClient()
        {
            if (this.CreateSmtpClientDelegate != null)
            {
                return this.CreateSmtpClientDelegate();
            }
            throw new NotImplementedException("only delegate implemented");
        }
        public override void SendNotification(IEnumerable<Notification> notifications)
        {
            Guard.ArgumentNotNull(notifications, nameof(notifications));
            using (SmtpClient client = CreateSmtpClient())
            {

                foreach (Notification n in notifications)
                {
                    MailMessage mm = CreateMailMessage(n);
                    client.Send(mm);
                }

            }
        }
    }




}
