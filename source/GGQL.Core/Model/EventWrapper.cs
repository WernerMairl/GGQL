using GGQL.Core.Internal;

namespace GGQL.Core
{
    public class EventWrapper : IEvent
    {
        public EventKey Key { get; private set; }
        public string JsonData { get; private set; }

        public EventWrapper(EventKey key, string jsonData)
        {
            Guard.ArgumentNotNull(key, nameof(key));
            Guard.ArgumentNotNull(jsonData, nameof(jsonData));
            this.Key = key;
            this.JsonData = jsonData;
        }

        EventKey IEvent.GetKey()
        {
            return this.Key;
        }

        string IEvent.GetJsonData()
        {
            return this.JsonData;
        }
    }
}