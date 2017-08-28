using System;
using SQLiteExtensions.Internal;

namespace SQLiteExtensions
{

    public class TableSnapshotBuilder : ITableSnapshotBuilder
    {
        private static ITableSnapshotBuilder _EmptySingleton;

        public static ITableSnapshotBuilder Empty
        {
            get
            {
                if (_EmptySingleton == null)
                {
                    TableSnapshotBuilder b = new TableSnapshotBuilder()
                    {
                        TableSnapshotDelegate = () => new EmptyTableSnapshot()
                     };
                    _EmptySingleton = b;
                }
                return _EmptySingleton;
            }
        }

        public Func<ITableSnapshot> TableSnapshotDelegate { get; set; }

        ITableSnapshot ITableSnapshotBuilder.Build()
        {
            if (this.TableSnapshotDelegate==null)
            {
                throw new InvalidOperationException("TableSnapshotDelegate");
            }
            return this.TableSnapshotDelegate();
        }
    }
}
