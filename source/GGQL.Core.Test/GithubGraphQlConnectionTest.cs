using Xunit;

namespace GGQL.Core.Test
{

    public class GithubGraphQlConnectionTest : TestWithToken
    {
        public GithubGraphQlConnectionTest():base("GithubToken")
        {}

        [Fact]
        public void DefaultIntrospectionTests()
        {
            GithubGraphQlConnection c = new GithubGraphQlConnection();
            string s = c.GetQueryResultAsync(this.Token, GGQL.Test.Resources.Resources.DefaultIntrospectionQuery).Result;

        }
    }
}
