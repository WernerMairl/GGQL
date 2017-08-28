using GGQL.Core.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GGQL.Core
{

    public class GithubGraphQlConnection
    {

        public async Task<string> GetQueryResultAsync(string token, string query)
        {
            Guard.ArgumentNotNullOrEmptyString(token, "token");
            Guard.ArgumentNotNullOrEmptyString(token, "query");
            using (HttpClient client = CreateHttpClient(token))
            {
                HttpContent content = new StringContent(GetPayload(query));
                var response = await client.PostAsync("https://api.github.com/graphql", content);
                var s1 = await response.Content.ReadAsStringAsync();
                return s1;
            }
        }

        private static HttpClient CreateHttpClient(string token)
        {
            var result = new HttpClient();
            //var auth = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            var auth = new AuthenticationHeaderValue("bearer", token);
            var userAgent = new ProductInfoHeaderValue("GGQL", "0.0.1");
            result.DefaultRequestHeaders.Authorization = auth;
            result.DefaultRequestHeaders.UserAgent.Add(userAgent);
            return result;
        }


        private static string GetPayload(string query)
        {
            var payload = new
            {
                Query = query,
            };

            string s = JsonConvert.SerializeObject(
                payload,
                Formatting.None,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return s;
        }

    }
}
