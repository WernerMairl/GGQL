using GGQL.Core.Internal;

namespace GGQL.Core
{
    //EventStore Core Requirements/Capabilities:
    //Filter Events by Target and/or Path (v2. part of Path)
    //Read events from a certain position
    // alternative: read events backward until stop.
    //use Timestamp to filter (forward, backward)

    //Question: Can or should we build a unique incremental value ?
    //Timestamp is incremental but not unique
    //GUID is unique but not incremental
    //both requirements together are causing a lot of effort and results in a high consistent reuslt. Probably we don't need that consistency
    //Attention: the Timestamp is created at the EventSource not by the Store, so the storage system is not ordered "by Timestamp"
    //Solution: EventStore is partitioned by Target and/or Path(Part of Path)


    public class TargetFilter
    {
        internal bool IsAllFilter { get; private set; }
        public static TargetFilter IncludeAll()
        {
            TargetFilter t = new TargetFilter(EmptyArray, EmptyArray) { IsAllFilter = true };
            return t;
        }

        public static TargetFilter SingleTarget(string target)
        {
            Guard.ArgumentNotNullOrEmptyString(target, nameof(target));
            return new TargetFilter(new string[] { target }, EmptyArray);
        }
        private TargetFilter(string[] includes, string[] excludes)
        {
            Guard.ArgumentNotNull(includes, nameof(includes));
            Guard.ArgumentNotNull(excludes, nameof(excludes));
            this.Include = includes;
            this.Exclude = excludes;
            this.IsAllFilter = false;
        }
        private static string[] EmptyArray = new string[] { };
        //Include one concrete Target
        //Include multiple concrete Targets
        //Include All Targets
        //NOT Supported: Targets with pattern matching => not usable for Partitions!

        public string[] Include { get; set; }

        public string[] Exclude { get; set; }

    }
}