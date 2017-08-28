using Microsoft.Data.Sqlite;
using SQLiteExtensions.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SQLiteExtensions
{
    /// <summary>
    /// Disposable => USING suggested!
    /// provides a Sqlite Database (in Memory)
    /// </summary>
    public class SqliteDatabaseProvider : IDisposable
    {

        public void CreateTable(ITableBuilder tableBuilder)
        {
            Guard.ArgumentNotNull(tableBuilder, nameof(tableBuilder));
            foreach (string statement in tableBuilder.Build())
            {
                this.Connection.Execute(statement); //execute create table and other DML Statements!
            }
        }


        public long BulkInsert(IEnumerable<string> inserts)
        {
            bool useTransaction = true;
            if (useTransaction)
            {
                this.Connection.Execute("BEGIN TRANSACTION");
            }

            try
            {
                long rowcounter = 0;
                foreach (string insertStatement in inserts)
                {
                    this.Connection.Execute(insertStatement);
                    rowcounter += 1;
                }
                return rowcounter;
            }
            finally
            {
                if (useTransaction)
                {
                    this.Connection.Execute("END TRANSACTION");
                }
            }
        }


        public bool IsInMemory { get; private set; }
        public static SqliteDatabaseProvider CreateInMemoryDatabase()
        {
            SqliteDatabaseProvider p = new SqliteDatabaseProvider("Data Source=:memory:", true);
            return p;
        }

        private void PerformanceSettings()
        {
            //https://stackoverflow.com/questions/1711631/improve-insert-per-second-performance-of-sqlite
            Guard.Assert(this.Connection.State == ConnectionState.Open);
            if (this.IsInMemory)
            {
                return;
            }
            //Attention we do this exactly for MICC-Scenario where we have high available servers/storage systems AND there is NO Problem if we lost a file!
            this.Connection.Execute("PRAGMA synchronous = OFF");
            this.Connection.Execute("PRAGMA journal_mode = MEMORY");
        }


        public SqliteConnection Connection { get; private set; }
        internal SqliteDatabaseProvider(string connectionString, bool isInMemory)
        {
            Guard.ArgumentNotNullOrEmptyString(connectionString, "connectionString");
            this.Connection = new SqliteConnection(connectionString);
            this.IsInMemory = isInMemory;
            this.Connection.Open();
            System.Diagnostics.Debug.WriteLine(string.Format("OpenConnection: {0}", this.Connection.ConnectionString));
            this.PerformanceSettings();
        }

        public static SqliteDatabaseProvider OpenOrCreateDatabase(string filename, ITableBuilder tableBuilder)
        {
            Guard.ArgumentNotNull(filename, nameof(filename));
            Guard.ArgumentNotNull(tableBuilder, nameof(tableBuilder));


            if (FileExistsAndNotEmpty(filename))
            {
                return OpenDatabase(filename);
            }
            else
            {
                SqliteDatabaseProvider p= CreateDatabase(filename);
                p.CreateTable(tableBuilder);
                return p;
            }

        }
        public static SqliteDatabaseProvider OpenDatabase(string filename)
        {
            Guard.ArgumentNotNull(filename, "filename");
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(string.Format("Database can't be opened, file '{0}' not exists", filename), filename);
            }

            SqliteConnectionStringBuilder sb = new SqliteConnectionStringBuilder
            {
                DataSource = filename
            };
            return new SqliteDatabaseProvider(sb.ToString(), false);
        }

        private static bool FileExistsAndNotEmpty(string filename)
        {
            Guard.ArgumentNotNull(filename, "filename");
            if (File.Exists(filename))
            {
                FileInfo fi = new FileInfo(filename);
                {
                    if (fi.Length == 0)
                    {
                        return false; //assume file not exists, probably created as temp file!
                    }
                }
                return true;
            }
            return false;
        }
        public static SqliteDatabaseProvider CreateDatabase(string filename)
        {
            Guard.ArgumentNotNull(filename, "filename");
            if (FileExistsAndNotEmpty(filename))
            {
                throw new InvalidOperationException(string.Format("Database can't be created, file '{0}' always exists", filename));
            }
            SqliteConnectionStringBuilder sb = new SqliteConnectionStringBuilder
            {
                DataSource = filename
            };
            return new SqliteDatabaseProvider(sb.ToString(), false);
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
                if (Connection != null)
                {
                    //we had some issue with not closed sqlite file. we found the following, but it was the wrong track!
                    //https://stackoverflow.com/questions/8511901/system-data-sqlite-close-not-releasing-database-file
                    //real reason: our differ was using some Reader with missing Dispose!!
                    //Connection.close does not close that handles!!
                    //so please ensure that ALL Resources coming from Connection are disposed properly
                    System.Diagnostics.Debug.WriteLine(string.Format("CloseConnection: {0}", this.Connection.ConnectionString));
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            }
        }


    }
}
