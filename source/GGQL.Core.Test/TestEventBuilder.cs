using System.Collections.Generic;

namespace GGQL.Core.Test
{
    public static class TestEventBuilder
    {
        public static readonly string FirstRepoName = "FirstRepo";
        public static readonly string SecondRepoName = "SecondRepo";
        public static readonly string FirstOwner = "FirstOwner";
        public static readonly string SecondOwner = "SecondOwner";


        public static IEvent[] FirstRepoCreatedEvent()
        {
            List<IEvent> events = new List<IEvent>();
            events.Add(new RepositoryEvent() { EventType = EventType.Created, IsPrivate = true, Name = FirstRepoName, Owner = FirstOwner, CreatedAt = System.DateTime.Now.AddHours(-1), UpdatedAt = System.DateTime.Now });
            return events.ToArray();
        }

            public static  IEvent[] CreateDemoEventMix()
        {
            List<IEvent> events = new List<IEvent>();
            events.Add(new RepositoryEvent() { EventType = EventType.Created, IsPrivate = true, Name = FirstRepoName, Owner = FirstOwner, CreatedAt = System.DateTime.Now.AddHours(-1), UpdatedAt = System.DateTime.Now });
            events.Add(new RepositoryEvent() { EventType = EventType.Modified, IsPrivate = false, Name = SecondRepoName, Owner = SecondOwner, CreatedAt = System.DateTime.Now.AddHours(-1), UpdatedAt = System.DateTime.Now });

            events.Add(new IssueEvent(FirstOwner, FirstRepoName) { EventType = EventType.Created, CreatedAt = System.DateTime.Now, Number = "77", Owner = FirstOwner, Repository = FirstRepoName, ResourcePath = "www.orf.at", State = "Closed", UpdatedAt = System.DateTime.Now });
            events.Add(new IssueEvent(FirstOwner, FirstRepoName) { EventType = EventType.Modified, CreatedAt = System.DateTime.Now, Number = "66", Owner = SecondOwner, Repository = SecondRepoName, ResourcePath = "www.orf.at", State = "Open", UpdatedAt = System.DateTime.Now });
            return events.ToArray();
        }
    }
}
