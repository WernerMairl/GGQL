namespace GGQL.Core
{

    public class RepositoryListDiffer
    {
        public string OldList { get; set; }
        public string NewList { get; set; }


      

        public void DoWork()
        {
            ////Tabellenstruktur ?
            //Dictionary<string, string> columns = new Dictionary<string, string>()
            //    .AddTextColumn("id")
            //    .AddTextColumn("name")
            //    .AddISO8601DateColumn("createdAt");
            //using (SqliteDatabaseProvider provider = SqliteDatabaseProvider.CreateInMemoryDatabase())
            //{
            //    //1. create (temp) table
            //    SqliteTableBuilder builder = new SqliteTableBuilder("RepositoryNodes") { Columns = columns, PrimaryKey = new string[] {"id"}};
            //    foreach (string statement in builder.Build())
            //    {
            //        provider.Connection.Execute(statement);
            //    }

            //    provider.BulkInsert(builder.GetInsertStatements(ExtractNodes(this.OldList)));
            //} //provider
        }






    }
}