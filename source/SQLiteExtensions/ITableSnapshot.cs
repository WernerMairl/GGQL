using System.Data;
using System;

namespace SQLiteExtensions
{
    /// <summary>
    /// WM 22.06.2017 we need IDisposable because it can be a Sqlite or any other storage technology.
    /// IDataRecord usage implicits some open Connection (disposable Resources). to many effort to make all that things offline. we can use sqlite-inMemory!
    /// </summary>
    public interface ITableSnapshot : IDisposable 
    {
        bool TryFindBrother(IDataRecord rowIdentfier, out IDataRecord result);
        IDataReader GetDataReader();
    }
}