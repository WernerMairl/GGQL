using System;
using System.Collections.Generic;

namespace GGQL.Core
{
    public class EventbasedCondition: Condition
    {
        //Die Condition sollte Events schon am EventStore filtern, durch Filtern auf den Typ
        //Die Condition sollte die Events schon am EventStore filtern durch angabe des Cursors: welches war das letzte event das gelesen wurde.



        public Func<IEvent,bool> FilterDelegate { get; set; }

        public override bool Resolve()
        {
            IEvent[] events = new IEvent[] { };
            List<IEvent> resolvedEvents = new List<IEvent>();
            foreach (IEvent se in events)
            {
                if (FilterDelegate(se))
                {
                    resolvedEvents.Add(se);
                }
            }
            return resolvedEvents.Count > 0;
        }

    }
}