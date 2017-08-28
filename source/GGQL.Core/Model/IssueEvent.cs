using Newtonsoft.Json;
using System;
using GGQL.Core.Internal;

namespace GGQL.Core
{

    public class IssueEvent : IEvent
    {

        public IssueEvent(string owner, string repository)
        {
            Guard.ArgumentNotNullOrEmptyString(owner, nameof(owner));
            Guard.ArgumentNotNullOrEmptyString(repository, nameof(repository));
            this.Owner = owner;
            this.Repository = repository;
        }

        public EventKey GetKey()
        {
            if (this.Owner == null)
            {
                throw new InvalidOperationException("Owner can't be empty/null");
            }

            if (this.Repository == null)
            {
                throw new InvalidOperationException("Repository can't be empty/null");
            }


            DateTime ts = this.CreatedAt;
            if (this.EventType != EventType.Created)
            {
                ts = this.UpdatedAt;
            }
            return new EventKey() {
                EventType = this.EventType,
                KeyTimestamp = ts,
                Path = string.Format(@"{0}\{1}\{2}",this.Owner.ToLowerInvariant(), this.Repository.ToLowerInvariant(), this.Number.ToLowerInvariant() ),
                Target = "Issue" };
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

        public string Repository { get; set; }

        public string Number { get; set; }

        public string ResourcePath { get; set; }
        public string State { get; set; }

    }
}