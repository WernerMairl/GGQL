using SQLiteExtensions.Internal;
using System.IO;

namespace SQLiteExtensions
{
    public static class SqliteTableSnapshotBuilder
    {

        public static ITableSnapshotBuilder FromSqliteDatabaseFile(string fileName, string tableName, params string[] primaryKey)
        {
            Guard.ArgumentNotNullOrEmptyString(fileName, "fileName");
            Guard.ArgumentNotNullOrEmptyString(tableName, "tableName");
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException(string.Format("Sqlite-Databasefile '{0}' not found!", fileName), fileName);
            }
            TableSnapshotBuilder b = new TableSnapshotBuilder() { TableSnapshotDelegate = () => OpenSqliteSnapshot(fileName, tableName, primaryKey) };
            return b;
        }


        private static ITableSnapshot OpenSqliteSnapshot(string fileName, string tableName, params string[] primaryKey)
        {
            Guard.AssertNotNullOrEmptyString(tableName);
            Guard.AssertNotNullOrEmptyString(fileName);
            SqliteDatabaseProvider p = SqliteDatabaseProvider.OpenDatabase(fileName);
            SqliteTableSnapshot sn = new SqliteTableSnapshot(p, tableName,primaryKey);
            return sn;
        }


        //public static ITableSnapshotBuilder FromJsonRpcMessage(JsonRpcMessage message, string tableName, params string[] primaryKey)
        //{
        //    Guard.ArgumentNotNull(message, "message");
        //    TableSnapshotBuilder b = new TableSnapshotBuilder() { TableSnapshotDelegate = () => CreateSqliteSnapshot(message, tableName, primaryKey) };
        //    return b;
        //}


        //private static ITableSnapshot CreateSqliteSnapshot(JsonRpcMessage msg, string tableName, SqliteDatabaseProvider databaseProvider, params string[] primaryKey)
        //{
        //    Guard.AssertNotNullOrEmptyString(tableName);
        //    Guard.Assert("ExecSQL" == msg.Method);
        //    string rpcresult = msg.GetResult<string>(new JsonSerializer());
        //    Ora2JsonResult res = new Ora2JsonResult(rpcresult);
        //    SqliteTableBuilder builder = new SqliteTableBuilder(tableName, res.Columns, primaryKey);
        //    string statement = builder.Build();

        //    databaseProvider.Connection.Execute(statement);
        //    int rowcounter = 0;
        //    foreach (string insertStatement in builder.GetInsertStatement(res.ReadRows()))
        //    {
        //        databaseProvider.Connection.Execute(insertStatement);
        //        rowcounter += 1;
        //    }

        //    Guard.Assert(rowcounter == databaseProvider.Connection.ExecuteScalarAsLong("SELECT COUNT(*) FROM " + tableName));

        //    SqliteTableSnapshot sn = new SqliteTableSnapshot(databaseProvider, builder.TableName, builder.PrimaryKey);
        //    return sn;

        //}



        //public static ITableSnapshotBuilder CreateSqliteSnapshotFile(JsonRpcMessage rpcMessage, string fileName, string tableName, params string[] primaryKey)
        //{
        //    Guard.ArgumentNotNullOrEmptyString(fileName, "fileName");
        //    Guard.ArgumentNotNullOrEmptyString(tableName, "tableName");
        //    Guard.ArgumentNotNull(rpcMessage, "rpcMessage");
        //    SqliteDatabaseProvider p = SqliteDatabaseProvider.CreateDatabase(fileName);
        //    TableSnapshotBuilder b = new TableSnapshotBuilder() { TableSnapshotDelegate = () => CreateSqliteSnapshot(rpcMessage, tableName, p, primaryKey) };
        //    return b;        
        //}



        //private static ITableSnapshot CreateSqliteSnapshot(JsonRpcMessage msg, string tableName, params string[] primaryKey)
        //{
        //    SqliteDatabaseProvider p = SqliteDatabaseProvider.CreateInMemoryDatabase();
        //    return CreateSqliteSnapshot(msg, tableName, p, primaryKey);
        //}


       

    }
}
