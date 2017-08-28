using System.Collections.Generic;
using SQLiteExtensions;
namespace GGQL.Core
{
    public class SimpleTableBuilder : ITableBuilder
    {
        public string[] Statements { get; private set; }
        public SimpleTableBuilder(params string[] statements)
        {
            this.Statements = statements;
            if (this.Statements == null)
            {
                this.Statements = new string[] { };
            }
        }
        IEnumerable<string> ITableBuilder.Build()
        {
            return Statements;
        }
    }
}
