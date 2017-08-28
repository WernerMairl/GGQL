using System;

namespace GGQL.Core
{
    public class TriggerBuilder
    {

        internal Func<Trigger> TriggerDelegate { get; set; }

        public static TriggerBuilder FireAlways()
        {
            TriggerBuilder tb = new TriggerBuilder();
            tb.TriggerDelegate = () =>
            {
                DelegatedTrigger t = new DelegatedTrigger();
                t.ShouldFireAtDelegate = (utcTime) => true;
                return t;
            };
            return tb;
        }

        public Trigger Build()
        {
            return null;
        }
    }
}