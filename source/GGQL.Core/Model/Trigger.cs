using System;

namespace GGQL.Core
{
    public abstract class Trigger
    {
        public abstract bool ShouldFireAt(DateTime utcTime);
    }
}