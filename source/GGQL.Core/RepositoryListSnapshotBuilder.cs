using GGQL.Core.Internal;
using Newtonsoft.Json.Linq;
using SQLiteExtensions;
using System;
using System.Collections.Generic;

namespace GGQL.Core
{

    public class RepositoryListSnapshotBuilder 
    {

        public static IEnumerable<Tuple<JObject, string>> ExtractNodes(IEnumerable<string> jsonRequests)
        {
            Guard.ArgumentNotNull(jsonRequests, nameof(jsonRequests));
            foreach (string json in jsonRequests)
            {
                JObject decoded = JObject.Parse(json);
                string owner = decoded["data"]["repositoryOwner"]["login"].ToString();
                foreach (JToken n in decoded["data"]["repositoryOwner"]["repositories"]["edges"])
                {
                    JObject node = n as JObject;
                    Guard.AssertNotNull(node);
                    JObject singleNode = (JObject)node["node"];
                    Tuple<JObject, string> res = new Tuple<JObject, string>(singleNode, owner);
                    Guard.AssertNotNullOrEmptyString(res.Item2);
                    yield return res;
                }
            }
        }

        public static SqliteTableBuilder CreateTable(SqliteDatabaseProvider provider)
        {
            Guard.ArgumentNotNull(provider, nameof(provider));
            SqliteTableBuilder builder = RepositoryTableSnapshot.CreateTableBuilder();
            provider.CreateTable(builder);
            return builder;
        }
        public static void Fill(SqliteDatabaseProvider provider, IEnumerable<string> jsonResponses)
        {
            //1. create (temp) table
            SqliteTableBuilder builder = CreateTable(provider);
            provider.BulkInsert(builder.GetInsertStatements(ExtractNodes(jsonResponses)));
        }

        public static ITableSnapshotBuilder CreateFileSnapshotFromRequest(string fileName, IEnumerable<string> jsonResponses)
        {
            Guard.ArgumentNotNullOrEmptyString(fileName, nameof(fileName));
            Guard.ArgumentNotNull(jsonResponses, nameof(jsonResponses));
            TableSnapshotBuilder b = new TableSnapshotBuilder();
            b.TableSnapshotDelegate = () =>
            {
                SqliteDatabaseProvider provider = SqliteDatabaseProvider.CreateDatabase(fileName);
                RepositoryListSnapshotBuilder.Fill(provider, jsonResponses);
                SqliteTableSnapshot ts = new SqliteTableSnapshot(provider, RepositoryTableSnapshot.TableName, RepositoryTableSnapshot.PrimaryKey);
                return ts;
            };
            return b;
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
                RepositoryListSnapshotBuilder.CreateTable(prov);
                SqliteTableSnapshot ts = new SqliteTableSnapshot(prov, RepositoryTableSnapshot.TableName, RepositoryTableSnapshot.PrimaryKey);
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
                RepositoryListSnapshotBuilder.Fill(prov, jsonResponses);

                SqliteTableSnapshot ts = new SqliteTableSnapshot(prov, RepositoryTableSnapshot.TableName, RepositoryTableSnapshot.PrimaryKey);
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
                SqliteTableSnapshot ts = new SqliteTableSnapshot(prov, RepositoryTableSnapshot.TableName, RepositoryTableSnapshot.PrimaryKey);
                return ts;
            };
            return b;
        }




        //ITableSnapshot ITableSnapshotBuilder.Build()
        //{
        //    SqliteDatabaseProvider prov = SqliteDatabaseProvider.CreateInMemoryDatabase();
        //    SqliteTableSnapshot ts = new SqliteTableSnapshot(prov, RepositoryTableSnapshot.TableName, RepositoryTableSnapshot.PrimaryKey);
        //    return ts;
        //}

    }
}