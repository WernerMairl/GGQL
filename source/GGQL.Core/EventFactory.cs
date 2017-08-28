using GGQL.Core.Internal;
using SQLiteExtensions;
using System.Data;

namespace GGQL.Core
{


    public static class EventFactory
    {

        public static IssueEvent CreateIssueEvent(IDataRecord template, EventType eventType)
        {
            Guard.ArgumentNotNull(template, nameof(template));
            string rp =template.ReadAsString( IssueTableSnapshot.ResourcePathFieldName);
            string[] splits = rp.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
            string owner = splits[0];
            string repository = splits[1];

            IssueEvent ev = new IssueEvent(owner, repository)
            {
                EventType = eventType,
                CreatedAt = template.ReadAsDateTime(IssueTableSnapshot.CreatedAtFieldName, enforceValue: true).Value,
                UpdatedAt = template.ReadAsDateTime(IssueTableSnapshot.UpdatedAtFieldName, enforceValue: true).Value,
                Number = template.ReadAsString(IssueTableSnapshot.NumberFieldName),
                State = template.ReadAsString("state"),
                ResourcePath = rp
                
            };
            return ev;

        }

        public static RepositoryEvent CreateRepositoryEvent(IDataRecord template, EventType eventType)
        {
            Guard.ArgumentNotNull(template, nameof(template));
            RepositoryEvent ev = new RepositoryEvent()
            {
                EventType = eventType,
                CreatedAt = template.ReadAsDateTime(RepositoryTableSnapshot.CreatedAtFieldName, enforceValue: true).Value,
                UpdatedAt = template.ReadAsDateTime(RepositoryTableSnapshot.UpdatedAtFieldName, enforceValue: true).Value,
                Name = template.ReadAsString("name"),
                IsPrivate = (template.ReadAsInt(RepositoryTableSnapshot.IsPrivateFieldName, enforceValue: true).Value != 0),
                Owner = template.ReadAsString("owner")
            };
            return ev;
        }
    }
}