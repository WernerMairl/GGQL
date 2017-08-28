namespace GGQL.Model
{
    public class Notification
    {
        public Notification()
        {
            this.Channels = NotificationChannel.EmptyArray;
            this.To = NotificationAddress.EmptyArray;
            this.MessageBody = string.Empty;
            this.Subject = string.Empty;
            this.IsHtml = false;
            this.Caption = string.Empty;
        }
        public NotificationAddress From { get; set; }
        public NotificationAddress[] To { get; set; }
        //public string CC { get; set; }
        //public string BCC { get; set; }
        public NotificationChannel[] Channels { get; set; } 

        public string MessageBody { get; set; }
        public string Subject { get; set; }

        public bool IsHtml { get; set; }

        public string Caption { get; set; }
    }

}
