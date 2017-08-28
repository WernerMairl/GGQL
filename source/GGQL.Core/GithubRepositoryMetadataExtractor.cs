using GGQL.Core.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GGQL.Core
{

    public class GithubRepositoryMetadataExtractor
    {
        //https://developer.github.com/v4/guides/resource-limitations/
        //Clients must supply a first or last argument on any connection.
        //Values of first and last must be within 1-100.
        //Individual calls cannot request more than 500,000 total nodes.
        private const int MaximumNodesCountPerRequestDefault = 100;


        public int MaximumNodesCountPerRequest { get; set; }


        public GithubRepositoryMetadataExtractor()
        {
            this.MaximumNodesCountPerRequest = MaximumNodesCountPerRequestDefault;
        }


        private string GetIssuesByRepositoryQuery(string owner, string repositoryName, string cursor, int maxNodes)
        {
            Guard.ArgumentNotNullOrEmptyString(owner, nameof(owner));
            Guard.ArgumentNotNullOrEmptyString(repositoryName, nameof(repositoryName));
            Guard.Assert(maxNodes >= 0);
            string restrict = string.Format("first: {0}", maxNodes);
            if (string.IsNullOrEmpty(cursor) == false)
            {
                Guard.Assert(cursor == cursor.Trim(), "trim");
                restrict += " after:\"" + cursor + "\"";
            }
            string s = IssueListPerRepositoryQueryTemplate.Replace("XXXXXXXXXX", "\"" + owner + "\"").Replace("YYYYYYYYYY", restrict).Replace("ZZZZZZZZZZ", "\"" + repositoryName + "\"");
            return s;
        }

        public static string GetRepositoriesByOwnerQuery(string login, string cursor, int maxNodes)
        {
            Guard.ArgumentNotNullOrEmptyString(login, "login");
            //we can't use string.format => to many {};
            string restrict = string.Format("first: {0}", maxNodes);
            if (string.IsNullOrEmpty(cursor) == false)
            {
                Guard.Assert(cursor == cursor.Trim(), "trim");
                restrict += " after:\"" + cursor + "\"";
            }

            string s = RepositorQueryTemplate.Replace("XXXXXXXXXX", "\"" + login + "\"").Replace("YYYYYYYYYY", restrict);

            return s;
        }



        //don't include bodytext => to many bytes/resources
        private static readonly string IssueListPerRepositoryQueryTemplate = @"
{
    repository(owner: XXXXXXXXXX name: ZZZZZZZZZZ) {
        issues(YYYYYYYYYY)
        {
            edges {
                node {
          id
          number
          state
          updatedAt
          createdAt
          resourcePath
            milestone {
                        id
            }
                    author {
                        login
                    }
                }
            }
            pageInfo {
                endCursor
                hasNextPage
            }
        }
    }
}

";


        private static readonly string RepositorQueryTemplate = @"
{
  repositoryOwner(login: XXXXXXXXXX) 
  {
    __typename
    login
    id
    repositories(YYYYYYYYYY)
        {
            edges
      {
                node
        {
                    id
                    name
                    isPrivate
                    createdAt
                    pushedAt
                    updatedAt
                }
            }
            pageInfo {
                endCursor
                hasNextPage
                }
        }
    }
}";

        private static string[] sep1 = new string[] { "\"pageInfo\"" };
        private static string[] sep2 = new string[] { "{", "}", ",", ":", "\"" };

        private void HandleCursor(string json, ref string cursor, ref bool hasNext)
        {
            Guard.AssertNotNullOrEmptyString(json);
            //we DON'T parse the Json for spead resons, we try to find the needed information at the fastest way!
            string[] splits = json.Split(sep1, StringSplitOptions.RemoveEmptyEntries);
            Guard.Assert(splits.Length == 2);
            string s = splits[1];

            string[] splits2 = s.Split(sep2, StringSplitOptions.RemoveEmptyEntries);
            Guard.Assert(splits2[0] == "endCursor");
            cursor = splits2[1];
            Guard.Assert(splits2[2] == "hasNextPage");
            string s1 = splits2[3];
            if (bool.TryParse(s1, out hasNext))
            {
                return;
            }
            throw new InvalidCastException("Problem with HasNextpage");
        }

        private IEnumerable<string> ExecuteQueryWithCursor(string token, Func<string, string> queryGetter)
        {
            Guard.AssertNotNull(queryGetter);
            string cursor = null;
            bool hasNextPage = true;
            //we must do all the things in a serialized way because the cursor enforces serialization!
            GithubGraphQlConnection connection = new GithubGraphQlConnection();
            while (hasNextPage)
            {
                //string resolvedQuery = GetRepositoriesByOwnerQuery(owner, cursor);
                string resolvedQuery = queryGetter(cursor);
                string s = connection.GetQueryResultAsync(token, resolvedQuery).Result;
                Guard.AssertNotNullOrEmptyString(s);
                HandleCursor(s, ref cursor, ref hasNextPage);
                yield return s;
            }
        }




        private IEnumerable<string> ReadRepositoryMetadata(string token, string owner, int maxNodes)
        {
            return ExecuteQueryWithCursor(token, (cursor) => GetRepositoriesByOwnerQuery(owner, cursor, maxNodes));
        }

        public async Task<IEnumerable<string>> GetMetadataAsynch(string token, string owner)
        {
            Func<IEnumerable<string>> impl = () => ReadRepositoryMetadata(token, owner, this.MaximumNodesCountPerRequest);
            return await Task.Factory.StartNew<IEnumerable<string>>(impl);
        }

        public async Task<IEnumerable<string>> GetIssueMetadataAsynch(string token, string owner, string repositoryName)
        {
            Guard.ArgumentNotNullOrEmptyString(token, nameof(token));
            Guard.ArgumentNotNullOrEmptyString(owner, nameof(owner));
            Guard.ArgumentNotNullOrEmptyString(repositoryName, nameof(repositoryName));
            Func<IEnumerable<string>> impl = () => ExecuteQueryWithCursor(token, (cursor) => GetIssuesByRepositoryQuery(owner, repositoryName, cursor, this.MaximumNodesCountPerRequest));
            return await Task.Factory.StartNew<IEnumerable<string>>(impl);
        }


    }
}
