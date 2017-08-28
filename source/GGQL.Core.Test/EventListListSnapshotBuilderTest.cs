using GGQL.Test.Resources;
using Newtonsoft.Json.Linq;
using SQLiteExtensions;
using Xunit;
namespace GGQL.Core.Test
{

    public class EventListListSnapshotBuilderTest
    {
        [Fact]
        public void Base_JsonInterpreter()
        {
            int n = (System.Linq.Enumerable.Count(EventListSnapshotBuilder.ExtractEventNodes("", "", new string[] { GGQL.Test.Resources.Resources.GetTRVEvents() })));
            Assert.Equal(25, n);
            foreach (System.Tuple<JObject, string> tpl in EventListSnapshotBuilder.ExtractEventNodes("", "", new string[] { GGQL.Test.Resources.Resources.GetTRVEvents() }))
            {
                Assert.True(tpl.Item1.Count > 5);
                Assert.False(string.IsNullOrEmpty(tpl.Item1["id"].ToString()));
                Assert.False(string.IsNullOrEmpty(tpl.Item1["name"].ToString()));
                Assert.False(string.IsNullOrEmpty(tpl.Item1["is_canceled"].ToString()));
                Assert.False(string.IsNullOrEmpty(tpl.Item1["start_time"].ToString()));

            }
        }

        [Fact]
        public void Base_Builder()
        {
            ITableSnapshot snapshot = EventListSnapshotBuilder.CreateInMemorySnapshotFromRequest(new string[] { Resources.GetTRVEvents() }).Build();
        }

    }
}
