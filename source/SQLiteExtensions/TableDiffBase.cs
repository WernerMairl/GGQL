using System;
using System.Data;

namespace SQLiteExtensions
{
    public delegate void TableDiffDelegate(IDataRecord deletedRecord, IDataRecord insertedRecord);

    public abstract class TableDiffBase
    {
        public TableDiffDelegate DifferencesDelegate { get; set; }

        protected void OnDifference(IDataRecord deletedRecord, IDataRecord insertedRecord)
        {
            DifferencesDelegate?.Invoke(deletedRecord, insertedRecord);
        }

        public abstract void Execute();

    }
}