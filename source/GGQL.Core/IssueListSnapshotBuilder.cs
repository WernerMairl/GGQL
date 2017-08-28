using GGQL.Core.Internal;
using Newtonsoft.Json.Linq;
using SQLiteExtensions;
using System;
using System.Collections.Generic;

namespace GGQL.Core
{

    public class IssueListSnapshotBuilder 
    {
        public static ITableSnapshotBuilder CreateFileSnapshotFromRequest(string fileName, IEnumerable<string> jsonResponses)
        {
            Guard.ArgumentNotNullOrEmptyString(fileName, nameof(fileName));
            Guard.ArgumentNotNull(jsonResponses, nameof(jsonResponses));
            TableSnapshotBuilder b = new TableSnapshotBuilder();
            b.TableSnapshotDelegate = () =>
            {
                SqliteDatabaseProvider provider = SqliteDatabaseProvider.CreateDatabase(fileName);
                Fill(provider, jsonResponses);
                SqliteTableSnapshot ts = new SqliteTableSnapshot(provider, IssueTableSnapshot.TableName, IssueTableSnapshot.PrimaryKey);
                return ts;
            };
            return b;
        }


        public static ITableSnapshotBuilder CreateFromFile(string fileName)
        {
            Guard.ArgumentNotNullOrEmptyString(fileName, nameof(fileName));
            if (System.IO.File.Exists(fileName) == false)
            {
                throw new System.IO.FileNotFoundException("Snapshotfile not found", fileName);
            }
            TableSnapshotBuilder b = new TableSnapshotBuilder();
            b.TableSnapshotDelegate = () =>
            {
                SqliteDatabaseProvider prov = SqliteDatabaseProvider.OpenDatabase(fileName);
                SqliteTableSnapshot ts = new SqliteTableSnapshot(prov, IssueTableSnapshot.TableName, IssueTableSnapshot.PrimaryKey);
                return ts;
            };
            return b;
        }



        public static SqliteTableBuilder CreateTable(SqliteDatabaseProvider provider)
        {
            Guard.ArgumentNotNull(provider, nameof(provider));
            SqliteTableBuilder builder = IssueTableSnapshot.CreateTableBuilder();
            provider.CreateTable(builder);
            return builder;
        }
        public static void Fill(SqliteDatabaseProvider provider, IEnumerable<string> jsonResponses)
        {
            //1. create (temp) table
            SqliteTableBuilder builder = CreateTable(provider);
            provider.BulkInsert(builder.GetInsertStatements(ExtractIssueNodes("owner","repo",jsonResponses)));
        }


        /// <summary>
        /// InMemory!
        /// </summary>
        /// <returns></returns>
        public static ITableSnapshotBuilder EmptySnapshot()
        {
            TableSnapshotBuilder b = new TableSnapshotBuilder();
            b.TableSnapshotDelegate = () =>
            {
                SqliteDatabaseProvider prov = SqliteDatabaseProvider.CreateInMemoryDatabase();
                IssueListSnapshotBuilder.CreateTable(prov);
                SqliteTableSnapshot ts = new SqliteTableSnapshot(prov, IssueTableSnapshot.TableName, IssueTableSnapshot.PrimaryKey);
                return ts;
            };
            return b;
        }

        public static ITableSnapshotBuilder CreateInMemorySnapshotFromRequest(IEnumerable<string> jsonResponses)
        {
            Guard.ArgumentNotNull(jsonResponses, nameof(jsonResponses));
            TableSnapshotBuilder b = new TableSnapshotBuilder();
            b.TableSnapshotDelegate = () =>
            {
                SqliteDatabaseProvider prov = SqliteDatabaseProvider.CreateInMemoryDatabase();
                IssueListSnapshotBuilder.Fill(prov, jsonResponses);

                SqliteTableSnapshot ts = new SqliteTableSnapshot(prov, IssueTableSnapshot.TableName, IssueTableSnapshot.PrimaryKey);
                return ts;
            };
            return b;
        }

        public static IEnumerable<Tuple<JObject, string>> ExtractIssueNodes(string owner, string repo, IEnumerable<string> jsonRequests)
        {
            Guard.ArgumentNotNull(jsonRequests, nameof(jsonRequests));
            foreach (string json in jsonRequests)
            {
                JObject decoded = JObject.Parse(json);
                string s1owner = string.Format(@"{0}/{1}", owner, repo);
                foreach (JToken n in decoded["data"]["repository"]["issues"]["edges"])
                {

                    JObject node = n as JObject;
                    Guard.AssertNotNull(node);
                    JObject singleNode = (JObject)node["node"];
                    Tuple<JObject, string> res = new Tuple<JObject, string>(singleNode, s1owner);
                    Guard.AssertNotNullOrEmptyString(res.Item2);
                    yield return res;



                }
            }
        }

    }
}