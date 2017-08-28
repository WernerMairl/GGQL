using System.Collections.Generic;

namespace GGQL.Model
{
    public abstract class NotificationChannel
    {
        public static NotificationChannel[] EmptyArray = new NotificationChannel[] { };

        //SendNotification Design Questions
        //Channel means email + smtp.port+credentials means one smtpclient instance
        public abstract void SendNotification(IEnumerable<Notification> notifications);  
    }


    
}
