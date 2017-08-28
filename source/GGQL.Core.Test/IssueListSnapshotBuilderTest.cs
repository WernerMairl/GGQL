using GGQL.Test.Resources;
using SQLiteExtensions;
using System.IO;
using Xunit;

namespace GGQL.Core.Test
{

    public class IssueListSnapshotBuilderTest
    {
        [Fact]
        public void Base_JsonInterpreter()
        {
            int counter = 0;
            foreach (var tpl in IssueListSnapshotBuilder.ExtractIssueNodes("myAccount", "myRepo", new string[] {Resources.GetIssuesResponse_FirstDraft()}))
            {
                TextWriter.Null.WriteLine(tpl.Item2);
                counter += 1;
            }
            Assert.Equal(15, counter); //adjusted from 20 to 15 at 11.08.2015
        }

        [Fact]
        public void Base_Builder()
        {
            ITableSnapshot snapshot = IssueListSnapshotBuilder.CreateInMemorySnapshotFromRequest(new string[] { Resources.GetIssuesResponse_FirstDraft() }).Build();
        }

    }
}
