using System;

namespace SQLiteExtensions
{
    public interface ITableSnapshotBuilder 
    {
        ITableSnapshot Build();
    }
}
