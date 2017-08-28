using System;

namespace GGQL.Core
{
    public class EventKey
    {
        public EventType EventType { get; set; }
        public DateTime KeyTimestamp { get; set; }
        /// <summary>
        /// Path basically is the UniqueKey for this event.
        /// UNIQUE per Target
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Issue, Repo.... (used to group liste of events)
        /// </summary>
        public string Target { get; set; }
    }
}