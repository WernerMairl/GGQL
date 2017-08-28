using System.Collections.Generic;
using System.Net.Mail;
using GGQL.Core.Internal;
using System.IO;
using GGQL.Core;

namespace GGQL.Model
{
    public class NotificationChannelsBuilder
    {
        public NotificationChannelsBuilder()
        {
            this._channels = new List<NotificationChannel>();
        }

        private List<NotificationChannel> _channels;
        public NotificationChannel[] Build()
        {
            return _channels.ToArray();
        }


        public NotificationChannelsBuilder UseSmtpPickupDirectory(string directory)
        {
            Guard.ArgumentNotNullOrEmptyString(directory, nameof(directory));
            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            SmtpClientNotificationChannel ch = new SmtpClientNotificationChannel();
            ch.CreateSmtpClientDelegate = () =>
            {
                SmtpClient cl = new SmtpClient();
                cl.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                cl.PickupDirectoryLocation = directory;
                return cl;
            };
            this._channels.Add(ch);
            return this;
        }
    }

}
