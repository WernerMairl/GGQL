using System.Collections.Generic;
using GGQL.Core.Internal;
using GGQL.Core;
using System.Linq;

namespace GGQL.Model
{
    public class NotificationBuilder
    {
        public NotificationRenderer Renderer { get; private set; }
        private List<NotificationChannel> _Channels;
        private List<IEvent> _Events;
        public NotificationBuilder AddEvent(IEvent singleEvent)
        {
            Guard.ArgumentNotNull(singleEvent, nameof(singleEvent));
            this._Events.Add(singleEvent);
            return this;
        }

        public NotificationBuilder UseSubject(string subject)
        {
            Guard.ArgumentNotNull(subject, nameof(subject));
            this.Subject = subject;
            return this;
        }
        public NotificationBuilder AddEvents(params IEvent[] events)
        {
            if (events == null)
            {
                return this;
            }
            foreach (IEvent se in events)
            {
                this._Events.Add(se);
            }
            return this;
        }



        public NotificationBuilder AddChannel(NotificationChannel channel)
        {
            Guard.ArgumentNotNull(channel, nameof(channel));
            this._Channels.Add(channel);
            return this;
        }


        public NotificationBuilder AddChannels(params NotificationChannel[] channels)
        {
            if (channels == null)
            {
                return this; //params keyword can reuslt in null that menas no items!
            }
            foreach (NotificationChannel ch in channels)
            {
                this._Channels.Add(ch);
            }
            return this;
        }


        public NotificationBuilder AddChannels(NotificationChannelsBuilder builder)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));
            return AddChannels(builder.Build());
        }
        public NotificationBuilder()
        {
            this._Channels = new List<NotificationChannel>();
            this._Events = new List<IEvent>();
            this.Renderer = new MarkdownNotificationRenderer();
            this.Subject = "";
        }

        public NotificationBuilder UseHtmlRenderer()
        {
            this.Renderer = new HtmlNotificationRenderer();
            return this;
        }

        public NotificationBuilder UseMarkdownRenderer()
        {
            this.Renderer = new MarkdownNotificationRenderer();
            return this;
        }


        public int EventCount
        {
            get
            {
                return _Events.Count;
            }
        }
        public bool HasEvents
        {
            get

            {
                return (_Events.Count > 0);
            }
        }

        public NotificationChannel[] Channels
        {
            get
            {
                return _Channels.ToArray();
            }
        }

        internal string Subject { get; private set; }

        public Notification Build()
        {
            Dictionary<string, List<IEvent>> groupedEvents = new Dictionary<string, List<IEvent>>();
            foreach (IEvent ev in this._Events)
            {
                EventKey k = ev.GetKey();
                string key = k.Target;
                List<IEvent> group = null;
                if (groupedEvents.TryGetValue(key, out group) == false)
                {
                    group = new List<IEvent>();
                    groupedEvents.Add(key, group);
                }
                Guard.AssertNotNull(group);
                group.Add(ev);
            }


            Notification n = new Notification();
            n.Channels = this.Channels;
            n.MessageBody = this.Renderer.BuildMessage(groupedEvents.Keys.ToArray(), (k1) => groupedEvents[k1]);
            n.Subject = this.Subject;
            n.IsHtml = true;
            return n;
        }

    }




}
