using SQLiteExtensions;
using System.Collections.Generic;

namespace GGQL.Core
{



    public class RepositoryTableSnapshot
    {

        public static SqliteTableBuilder CreateTableBuilder()
        {
            return new SqliteTableBuilder(RepositoryTableSnapshot.TableName) { PrimaryKey = RepositoryTableSnapshot.PrimaryKey, Columns = RepositoryTableSnapshot.Columns };
        }

        public static readonly string[] PrimaryKey = new string[] { "id" };
        public static readonly string TableName = "Repositories";
        public static readonly string CreatedAtFieldName = "createdAt";
        public static readonly string IsPrivateFieldName = "isPrivate";
        public static readonly string UpdatedAtFieldName = "updatedAt";
        public static readonly IDictionary<string, string> Columns = new Dictionary<string, string>()
                       .AddTextColumn("id")
                       .AddTextColumn("name")
                       .AddTextColumn("owner")
                       .AddBooleanColumn(IsPrivateFieldName) 
                       .AddISO8601DateColumn(CreatedAtFieldName)
                       .AddISO8601DateColumn(UpdatedAtFieldName)
            ;

    }
}