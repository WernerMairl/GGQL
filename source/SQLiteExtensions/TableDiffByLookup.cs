using System;
using System.Data;
using SQLiteExtensions.Internal;

namespace SQLiteExtensions
{
    public class TableDiffByLookup : TableDiffBase
    {
        public TableDiffByLookup(ITableSnapshot oldTable, ITableSnapshot newTable)
        {
            this.OldSnapshot = oldTable;
            this.NewSnapshot = newTable;
            this.OldSnapshotRecordCount = -1;
            this.NewSnapshotRecordCount = -1;
        }
        public int OldSnapshotRecordCount { get; private set; }
        public int NewSnapshotRecordCount { get; private set; }
        public ITableSnapshot OldSnapshot { get; private set; }
        public ITableSnapshot NewSnapshot { get; private set; }


        private void TryDispose(IDataRecord r)
        {
            Guard.AssertNotNull(r);
            IDisposable d = r as IDisposable;
            if (d != null)
            {
                d.Dispose();
            }

        }
        public override void Execute()
        {
            Guard.AssertNotNull(this.OldSnapshot);
            Guard.AssertNotNull(this.NewSnapshot);
            Guard.Assert(this.OldSnapshot != this.NewSnapshot, "not tested for sideeffects");
            using (IDataReader oldReader = this.OldSnapshot.GetDataReader()) //should be sorted, but not required!
            {
                this.OldSnapshotRecordCount = 0;
                while (oldReader.Read())
                {
                    this.OldSnapshotRecordCount += 1;
                    if (this.NewSnapshot.TryFindBrother(oldReader, out IDataRecord brother))
                    {
                        Guard.AssertNotNull(brother);
                        if (oldReader.IsTheSame(brother) == false)
                        {
                            //Update on some place outside the PK
                            OnDifference(oldReader, brother);
                        }
                        else
                        {
                            //NO CHANGES!
                        }
                        TryDispose(brother);
                    }
                    else
                    {
                        OnDifference(oldReader, null);
                    }
                }

            }

            using (IDataReader newReader = this.NewSnapshot.GetDataReader())
            {
                this.NewSnapshotRecordCount = 0;
                while (newReader.Read())
                {
                    this.NewSnapshotRecordCount += 1;
                    if (this.OldSnapshot.TryFindBrother(newReader, out IDataRecord brother))
                    {
                        Guard.AssertNotNull(brother);
                        TryDispose(brother);
                        //we must ignore Updates here, to prevent double detection!
                        continue;
                    }
                    else
                    {
                        Guard.AssertNull(brother);// "if not we must dispose");
                        OnDifference(null, newReader);
                    }
                }

            }



        }

    }
}