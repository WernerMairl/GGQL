using GGQL.Core.Internal;
using Newtonsoft.Json.Linq;
using SQLiteExtensions;
using System;
using System.Collections.Generic;

namespace GGQL.Core
{

    public class EventListSnapshotBuilder
    {
        public static ITableSnapshotBuilder CreateInMemorySnapshotFromRequest(IEnumerable<string> jsonResponses)
        {
            Guard.ArgumentNotNull(jsonResponses, nameof(jsonResponses));
            TableSnapshotBuilder b = new TableSnapshotBuilder();
            b.TableSnapshotDelegate = () =>
            {
                SqliteDatabaseProvider prov = SqliteDatabaseProvider.CreateInMemoryDatabase();
                Fill(prov, jsonResponses);
                SqliteTableSnapshot ts = new SqliteTableSnapshot(prov, EventTableSnapshot.TableName, EventTableSnapshot.PrimaryKey);
                return ts;
            };
            return b;
        }

        public static SqliteTableBuilder CreateTable(SqliteDatabaseProvider provider)
        {
            Guard.ArgumentNotNull(provider, nameof(provider));
            SqliteTableBuilder builder = EventTableSnapshot.CreateTableBuilder();
            provider.CreateTable(builder);
            //foreach (string statement in builder.Build())
            //{
            //    provider.Connection.Execute(statement); //execute create table and other DML Statements!
            //}
            return builder;
        }
        public static void Fill(SqliteDatabaseProvider provider, IEnumerable<string> jsonResponses)
        {
            //1. create (temp) table
            SqliteTableBuilder builder = CreateTable(provider);
            provider.BulkInsert(builder.GetInsertStatements(ExtractEventNodes("owner", "repo", jsonResponses)));
        }

        public static IEnumerable<Tuple<JObject, string>> ExtractEventNodes(string owner, string repo, IEnumerable<string> jsonRequests)
        {
            Guard.ArgumentNotNull(jsonRequests, nameof(jsonRequests));
            foreach (string json in jsonRequests)
            {
                JObject decoded = JObject.Parse(json);
                //string s1owner = string.Format(@"{0}/{1}", owner, repo);
                foreach (JToken n in decoded["events"]["data"])
                {
                    JObject node = n as JObject;
                    Guard.AssertNotNull(node);
                    Tuple<JObject, string> res = new Tuple<JObject, string>(node, owner);
                    //Guard.AssertNotNullOrEmptyString(res.Item2);
                    yield return res;
                }
            }
        }

    }
}