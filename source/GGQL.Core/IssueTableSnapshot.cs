using SQLiteExtensions;
using System.Collections.Generic;

namespace GGQL.Core
{



    public class IssueTableSnapshot
    {

        public static SqliteTableBuilder CreateTableBuilder()
        {
            return new SqliteTableBuilder(IssueTableSnapshot.TableName) { PrimaryKey = IssueTableSnapshot.PrimaryKey, Columns = IssueTableSnapshot.Columns };
        }

        public static readonly string[] PrimaryKey = new string[] { "id" };
        public static readonly string TableName = "Issues";
        public static readonly string CreatedAtFieldName = "createdAt";
        public static readonly string UpdatedAtFieldName = "updatedAt";
        public static readonly string NumberFieldName = "number";
        public static readonly string ResourcePathFieldName = "resourcePath";

        //id
        //number
        //bodyText
        //state
        //updatedAt
        //createdAt
        //resourcePath

        public static readonly IDictionary<string, string> Columns = new Dictionary<string, string>()
                       .AddTextColumn("id")
                       .AddTextColumn(NumberFieldName)
                       .AddTextColumn("state")
                       .AddTextColumn(ResourcePathFieldName) 
                       .AddISO8601DateColumn(CreatedAtFieldName)
                       .AddISO8601DateColumn(UpdatedAtFieldName);
    }
}