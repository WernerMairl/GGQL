using Xunit;
using Xunit.Abstractions;
namespace GGQL.Core.Test
{
    public class NotificationRendererTest
    {
        public ITestOutputHelper Helper { get; private set; }
        public NotificationRendererTest(ITestOutputHelper helper)
        {
            this.Helper = helper;
        }
        [Fact]
        public void HtmlBaseTest()
        {
        }
    }
}
