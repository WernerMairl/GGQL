using System;
using GGQL.Core.Internal;
using GGQL.Core;
using GGQL.Model;

namespace GGQL.Core
{

    

    public class WorkDescription
    {
        public string AccessToken { get; set; }
        public WorkItemType ItemType { get; set; }

        public SnapshotRepository Repository { get; private set; }

        public WorkDescription(SnapshotRepository repository, WorkItemType itemType)
        {
            Guard.ArgumentEnumValueIsDefined<WorkItemType>(itemType, nameof(itemType));
            this.Repository = repository;
            this.Items = new string[] { };
            this.Channels = new NotificationChannel[] { };
            this.From = null;
            this.To = new NotificationAddress[] { };
            this.ItemType = itemType;
        }
        public string[] Items { get; set; }

        public string GetKey(string item)
        {
            Guard.ArgumentNotNullOrEmptyString(item, nameof(item));
            item = item.Replace('/', '_').Replace( '\\' , '_');
            string key = string.Format("{0}_{1}",this.ItemType.ToString(),item);
            return key;
        }

        public NotificationChannel[] Channels { get; set; }
        public NotificationAddress From { get; set; }
        public NotificationAddress[] To { get; set; }
    }
}
