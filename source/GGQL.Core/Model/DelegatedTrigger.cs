using System;

namespace GGQL.Core
{
    public class DelegatedTrigger : Trigger
    {
        public Func<DateTime,bool> ShouldFireAtDelegate { get; set; }
        public override bool ShouldFireAt(DateTime utcTime)
        {
            if (this.ShouldFireAtDelegate == null)
            {
                throw new InvalidOperationException("Delegate not defined");
            }
            return this.ShouldFireAtDelegate(utcTime);
        }

    }
}