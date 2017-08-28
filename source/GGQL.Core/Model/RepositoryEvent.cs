using GGQL.Core.Internal;
using Newtonsoft.Json;
using System;

namespace GGQL.Core
{

    public class RepositoryEvent : IEvent
    {

        public EventKey GetKey()
        {
            DateTime ts = this.CreatedAt;
            Guard.Assert(this.Owner.Trim() == this.Owner, "trim_001");
            Guard.Assert(this.Name.Trim() == this.Name, "trim_002");
            if (this.EventType != EventType.Created)
            {
                ts = this.UpdatedAt;
            }
            return new EventKey() {
                EventType = this.EventType,
                KeyTimestamp = ts,
                Path = string.Format(@"{0}\{1}", this.Owner.ToLowerInvariant(), this.Name.ToLowerInvariant()),
                Target = "Repository"
            };
        }

        public string GetJsonData()
        {
            string s = JsonConvert.SerializeObject(this);
            return s;
        }

        public EventType EventType { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Owner { get; set; }

        public string Name { get; set; }

        public bool IsPrivate { get; set; }

    }
}