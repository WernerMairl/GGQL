using Microsoft.Data.Sqlite;
using SQLiteExtensions.Internal;

namespace SQLiteExtensions
{
    public static class SqliteConnectionExtensions
    {

        public static long ExecuteScalarAsLong(this SqliteConnection connection, string commandText)
        {
            Guard.ArgumentNotNull(connection, nameof(connection));
            Guard.ArgumentNotNullOrEmptyString(commandText, nameof(commandText));
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            using (SqliteDataReader r = command.ExecuteReader())
            {
                if (r.Read())
                {
                    return r.GetInt64(0);
                }
                throw new System.InvalidOperationException("Scalar without result");
            }
        }



        public static SqliteDataReader ExecuteReader(this SqliteConnection connection, string commandText)
        {
            Guard.ArgumentNotNull(connection, nameof(connection));
            Guard.ArgumentNotNullOrEmptyString(commandText, nameof(commandText));
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            SqliteDataReader r = command.ExecuteReader();
            Guard.Assert(r.IsClosed == false, "not expected IsClosedState on SqliteDataReader");
            return r;
        }

        public static void Execute(this SqliteConnection connection, string commandText)
        {
            Guard.ArgumentNotNull(connection, nameof(connection));
            Guard.ArgumentNotNullOrEmptyString(commandText, nameof(commandText));
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }
    }
}