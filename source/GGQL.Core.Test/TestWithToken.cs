using Xunit;

namespace GGQL.Core.Test
{
    /// <summary>
    /// Abstract Base Class for other Tests using a GitHub Token
    /// </summary>
    public abstract class TestWithToken
    {
        public string Token { get; protected set; }
        public TestWithToken(string environmentVariableName)
        {
            if (string.IsNullOrEmpty(environmentVariableName))
            {
                throw new System.ArgumentException("message", nameof(environmentVariableName));
            }

            string token = System.Environment.GetEnvironmentVariable(environmentVariableName);
            Assert.False(string.IsNullOrEmpty(token), "token not defined");
            this.Token = token;
        }
    }
}
