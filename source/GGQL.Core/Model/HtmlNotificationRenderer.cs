using GGQL.Core.Internal;
using Markdig;
using System;
using System.Collections.Generic;

namespace GGQL.Core
{
    public class HtmlNotificationRenderer : NotificationRenderer
    {
        private string ConvertMarkdownToHtml(string markdownContent)
        {
            Guard.ArgumentNotNullOrEmptyString(markdownContent, nameof(markdownContent));
            var pipeline = new MarkdownPipelineBuilder();//.UseAdvancedExtensions();
            pipeline.UsePipeTables();
            //pipeline.UseGridTables();
            string result = Markdown.ToHtml(markdownContent, pipeline.Build());
            return result;
        }


        public override string BuildMessage(string[] eventGroups, Func<string, IEnumerable<IEvent>> groupGetter)
        {
            MarkdownNotificationRenderer md = new MarkdownNotificationRenderer();

            return this.ConvertMarkdownToHtml(md.BuildMessage(eventGroups,groupGetter));
        }
    }




}
