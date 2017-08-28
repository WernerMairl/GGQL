using GGQL.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace GGQL.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownNotificationRenderer : NotificationRenderer
    {
        public override string BuildMessage(string[] eventGroups, Func<string, IEnumerable<IEvent>> groupGetter)
        {
            Guard.AssertNotNull(this.Configuration);
            MarkdownNotificationRendererSettings settings = new MarkdownNotificationRendererSettings();
            Guard.ArgumentNotNull(eventGroups, nameof(eventGroups));
            Guard.ArgumentNotNull(groupGetter, nameof(groupGetter));
            System.Reflection.Assembly th = typeof(MarkdownNotificationRenderer).Assembly;

            StringBuilder sb = new StringBuilder();

            if (settings.ShowHeader)
            {
                sb.AppendLine(string.Format("# GGQL Notification"));
            }
            string[] columnNames = new string[] { "Owner", "Repository", "Issue", "Event", "Url" };
            string[] headerLine = new string[] { "---", "---", "---", "---", "---" };
            Guard.Assert(headerLine.Length == columnNames.Length);
            foreach (string eg in eventGroups)
            {
                if (settings.ShowGroupHeader)
                {
                    sb.AppendLine(string.Format("## {0}", eg));
                }

                if (eg.ToLowerInvariant() == "issue")
                {
                    string line1 = string.Join("|", columnNames);
                    string line2 = string.Join("|", headerLine);

                    sb.AppendLine(line1);
                    sb.AppendLine(line2);

                    foreach (IEvent e1 in groupGetter(eg))
                    {
                        IssueEvent ie = e1 as IssueEvent;
                        {
                            string uri = string.Format(@"https://github.com{0}", ie.ResourcePath);
                            string[] rowContent = new string[] { ie.Owner, ie.Repository, ie.Number, ie.EventType.ToString(), uri };
                            Guard.Assert(rowContent.Length == columnNames.Length);
                            string dataLine = String.Join("|", rowContent);
                            sb.AppendLine(dataLine);
                        }
                    }

                }
                else
                {

                    string rowTemplate = "{0}|{1}|{2}|{3}";
                    sb.AppendLine(string.Format(rowTemplate, "Owner", "Repository", "Event", "Url"));
                    sb.AppendLine(string.Format(rowTemplate, "---", "---", "---", "---"));

                    foreach (IEvent e1 in groupGetter(eg))
                    {
                        RepositoryEvent re = e1 as RepositoryEvent;
                        if (re != null)
                        {
                            string uri = string.Format(@"https://github.com/{0}/{1}/", re.Owner, re.Name);
                            sb.AppendLine(string.Format(rowTemplate, re.Owner, re.Name, re.EventType, uri));
                        }
                    }
                } //not issue
                if (settings.ShowGroupHeader)
                {
                    sb.AppendLine(); //group footer
                }
            } //foreach eventGroup
            sb.AppendLine("");
            sb.AppendLine(string.Format("<!-- Created with GGQL NotificationBuilder v {0} --> ", Helper.GetProductVersion(th)));
            return sb.ToString();
        }

    }




}
