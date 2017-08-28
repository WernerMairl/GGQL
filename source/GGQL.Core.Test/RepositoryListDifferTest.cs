using Xunit;
using GGQL.Test.Resources;
using SQLiteExtensions;
using System;

namespace GGQL.Core.Test
{
    public class RepositoryListDifferTest : IDisposable
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
                if (this.DatabaseProvider != null)
                {
                    this.DatabaseProvider.Dispose();
                    this.DatabaseProvider = null;
                }
            }
        }

        public RepositoryListDifferTest()
        {
            this.DatabaseProvider = SqliteDatabaseProvider.CreateInMemoryDatabase();
        }
        public SqliteDatabaseProvider DatabaseProvider { get; private set; }





        [Fact]
        public void Repository_Diff()
        {

            ITableSnapshotBuilder builder;

            builder =  RepositoryListSnapshotBuilder.CreateInMemorySnapshotFromRequest(Resources.GetRepositoryResponse("Microsoft", RepositoryResponseGeneration.July));
            ITableSnapshot microsoftSnapshotJuly = builder.Build();

            builder = RepositoryListSnapshotBuilder.CreateInMemorySnapshotFromRequest(Resources.GetRepositoryResponse("Xamarin", RepositoryResponseGeneration.July));
            ITableSnapshot xamarinSnapshotJuly = builder.Build();

            TableDiffByLookup differ = new TableDiffByLookup(microsoftSnapshotJuly, xamarinSnapshotJuly);
            differ.DifferencesDelegate = (deletedRecord, inserted) =>
            {
                System.Diagnostics.Debug.WriteLine("");
            };
            differ.Execute();
            Assert.Equal(136, differ.NewSnapshotRecordCount);
            Assert.Equal(1312, differ.OldSnapshotRecordCount);
        }
    }
}
