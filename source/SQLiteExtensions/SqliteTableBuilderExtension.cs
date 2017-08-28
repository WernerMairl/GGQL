using SQLiteExtensions.Internal;
using System.Collections.Generic;

namespace SQLiteExtensions
{
    public static class SqliteTableBuilderExtension
    {


        public static Dictionary<string, string> AddBooleanColumn(this Dictionary<string, string> columns, string columnName)
        {
            Guard.ArgumentNotNull(columns, nameof(columns));
            Guard.ArgumentNotNullOrEmptyString(columnName, nameof(columnName));
            columns.Add(columnName, "INTEGER"); //0/1
            return columns;
        }

        public static Dictionary<string, string> AddTextColumn(this Dictionary<string, string> columns, string columnName)
        {
            Guard.ArgumentNotNull(columns, nameof(columns));
            Guard.ArgumentNotNullOrEmptyString(columnName, nameof(columnName));
            columns.Add(columnName, "TEXT");
            return columns;
        }

        public static Dictionary<string, string> AddISO8601DateColumn(this Dictionary<string, string> columns, string columnName)
        {
            Guard.ArgumentNotNull(columns, nameof(columns));
            Guard.ArgumentNotNullOrEmptyString(columnName, nameof(columnName));
            columns.Add(columnName, "TEXT");
            return columns;
        }



    }
}