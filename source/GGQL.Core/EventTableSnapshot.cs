using SQLiteExtensions;
using System.Collections.Generic;

namespace GGQL.Core
{
    public class EventTableSnapshot
    {
        public static SqliteTableBuilder CreateTableBuilder()
        {
            return new SqliteTableBuilder(EventTableSnapshot.TableName) { PrimaryKey = EventTableSnapshot.PrimaryKey, Columns = EventTableSnapshot.Columns };
        }

        public static readonly string[] PrimaryKey = new string[] { "id" };
        public static readonly string TableName = "Events";
        public static readonly string StartTimeFieldName = "start_time";
        public static readonly string IsCanceledFieldName = "is_canceled";

        public static readonly IDictionary<string, string> Columns = new Dictionary<string, string>()
                       .AddTextColumn("id")
                       .AddTextColumn("name")
                       .AddBooleanColumn(IsCanceledFieldName)
                       //.AddTextColumn(ResourcePathFieldName)
                       //.AddISO8601DateColumn(CreatedAtFieldName)
                       .AddISO8601DateColumn(StartTimeFieldName);
    }
}