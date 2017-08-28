using SQLiteExtensions.Internal;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;

namespace SQLiteExtensions
{
    /// <summary>
    /// Snapshot is without Storage Layer!
    /// the provided databaseProvider is DISPOSED!
    /// </summary>
    public class SqliteTableSnapshot : ITableSnapshot
    {

        internal SqliteDatabaseProvider DatabaseProvider { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseProvider">attention: dispose of the snapshot disposes the provider!</param>
        /// <param name="tableName"></param>
        /// <param name="primaryKey"></param>
        public SqliteTableSnapshot(SqliteDatabaseProvider databaseProvider, string tableName, params string[] primaryKey)
        {
            Guard.ArgumentNotNull(databaseProvider, "databaseProvider");
            Guard.ArgumentNotNullOrEmptyString(tableName, "tableName");
            Guard.ArgumentNotNullOrEmptyList(primaryKey, "primaryKey");
            this.DatabaseProvider = databaseProvider;
            this.TableName = tableName;
            this.PrimaryKey = primaryKey;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.DatabaseProvider != null)
                {
                    this.DatabaseProvider.Dispose();
                    this.DatabaseProvider = null;
                }
            }
        }


        public string TableName { get; private set; }

        public string[] PrimaryKey { get; private set; }

        public SqliteConnection Connection
        {
            get
            {
                Guard.AssertNotNull(this.DatabaseProvider);
                return this.DatabaseProvider.Connection;
            }
        }
        public IDataReader GetDataReader()
        {
            return this.Connection.ExecuteReader(string.Format("Select * from {0}", this.TableName));
        }

        public bool TryFindBrother(IDataRecord rowIdentfier, out IDataRecord result)
        {
            result = null;
            string sql = string.Format("Select * from {0} ", this.TableName);
            List<string> restrictions = new List<string>();

            foreach (string pkf in this.PrimaryKey)
            {
                int i = rowIdentfier.GetOrdinal(pkf);
                Guard.Assert(i >= 0);
                System.Type t = rowIdentfier.GetFieldType(i);
                restrictions.Add(string.Format("{0}={1}", pkf, SqliteExpression.GetExpressionString(rowIdentfier.GetValue(i), t)));
            }
            Guard.Assert(restrictions.Count > 0);
            string where = string.Join(" AND ", restrictions);
            string resolved = sql + "WHERE " + where;
            bool enforceDispose = true;
            IDataReader r = this.Connection.ExecuteReader(resolved);
            try
            {
                bool b = r.Read();
                if (b)
                {
#if DEBUG
                    long recordcount = this.Connection.ExecuteScalarAsLong(string.Format("SELECT COUNT(*) FROM {0} WHERE {1}", this.TableName, where));
                    Guard.Assert(recordcount == 1, "extly one expected");
#endif
                    enforceDispose = false;
                    result = r;
                    return true;
                }
                else
                {
                    result = null;
                    Guard.Assert(enforceDispose);
                    return false;
                }
            }
            finally
            {
                if (enforceDispose)
                {
                    r.Dispose();
                }
            }
        }
    }
}