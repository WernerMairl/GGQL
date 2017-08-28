using System.Collections.Generic;
namespace SQLiteExtensions
{
    public interface ITableBuilder
    {
        /// <summary>
        /// each string/statement is executed separately
        /// if a execution throws a exception then the enumeration is canceled!
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> Build();
    }
}