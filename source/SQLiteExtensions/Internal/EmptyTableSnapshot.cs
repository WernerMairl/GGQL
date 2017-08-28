using System;
using System.Data;
using SQLiteExtensions;
namespace SQLiteExtensions.Internal
{

    public class EmptyTableSnapshot : ITableSnapshot
    {
       

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        IDataReader ITableSnapshot.GetDataReader()
        {
            return new Internal.DummyDbDataReader();
        }

        bool ITableSnapshot.TryFindBrother(IDataRecord rowIdentfier, out IDataRecord result)
        {
            result = null;
            return false;
        }
    }
}