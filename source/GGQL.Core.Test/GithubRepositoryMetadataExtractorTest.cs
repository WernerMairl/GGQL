using GGQL.Model;
using GGQL.Test.Resources;
using SQLiteExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Xunit;

namespace GGQL.Core.Test
{

    /// <summary>
    /// THIS Test Class extecutes (=DEPENDENCY) REST Calls on www.github.com. A Token is required!
    /// </summary>
    public class GithubRepositoryMetadataExtractorTest : TestWithToken
    {

        public GithubRepositoryMetadataExtractor Extractor { get; private set; }
        public string TargetDirectory { get; private set; }
        public GithubRepositoryMetadataExtractorTest() : base("GithubToken")
        {
            this.TargetDirectory = @"c:\work2";
            if (Directory.Exists(this.TargetDirectory) == false)
            {
                Directory.CreateDirectory(this.TargetDirectory);
            }
            this.Extractor = new GithubRepositoryMetadataExtractor();
        }

        [Theory]
        [InlineData("Microsoft")]
        [InlineData("Facebook")]
        [InlineData("Dotnet")]
        [InlineData("Xamarin")]
        public void Extract_Repos_By_Owner(string owner)
        {
            int counter = 0;
            string ts = DateTime.Now.ToString("yyyyMMddHHmmss");
            foreach (string json in this.Extractor.GetMetadataAsynch(this.Token, owner).Result)
            {
                string fn = string.Format("{0}_{1}_{2}.json",ts, counter , owner);
                string fullName = Path.Combine(this.TargetDirectory, fn);
                File.WriteAllText(fullName, json);
                counter += 1;
            }
        }


        [Theory]
        [InlineData("Dotnet", "cli")]
        [InlineData("Microsoft", "vscode")]

        public void Extract_Issues_By_Repo(string owner, string repo)
        {
            int counter = 0;
            string ts = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.Extractor.MaximumNodesCountPerRequest = 15;
            foreach (string json in this.Extractor.GetIssueMetadataAsynch(this.Token, owner, repo).Result)
            {
                string fn = string.Format("Issues_{0}_{1}_{2}.json", ts, counter, owner);
                string fullName = Path.Combine(this.TargetDirectory, fn);
                File.WriteAllText(fullName, json);
                int cnt = System.Linq.Enumerable.Count(IssueListSnapshotBuilder.ExtractIssueNodes(owner,repo, new string[] { json }));
                Assert.Equal(this.Extractor.MaximumNodesCountPerRequest, cnt);
                counter += 1;
                if (counter > 2) break;
            }
        }

        [Theory]
        [InlineData("Dotnet", "core")] //small repo
        //[InlineData("Microsoft", "vscode")]
        public void Issue_Snapshot_from_Web(string owner, string repo)
        {
            ITableSnapshotBuilder builder;
            Assert.True(this.Extractor.MaximumNodesCountPerRequest > 50);
            builder = IssueListSnapshotBuilder.CreateInMemorySnapshotFromRequest(this.Extractor.GetIssueMetadataAsynch(this.Token, owner, repo).Result);
            using (ITableSnapshot snapshot = builder.Build())
            {
                using (var reader = snapshot.GetDataReader())
                {
                    while (reader.Read())
                    {
                        EventFactory.CreateIssueEvent(reader, EventType.Created);
                    }
                }
            }
        }





        [Theory]
        [InlineData("Microsoft")]
        [InlineData("Xamarin")]

        public void Compare_Repos_July_vs_Today(string owner)
        {

            NotificationBuilder nbuilder = new NotificationBuilder();
            NotificationChannelsBuilder channelsBuilder = new NotificationChannelsBuilder().UseSmtpPickupDirectory(@"c:\work").UseSmtpPickupDirectory(@"c:\work2\send");

            List<RepositoryEvent> Events = new List<RepositoryEvent>();
            ITableSnapshotBuilder builder;

            builder = RepositoryListSnapshotBuilder.CreateInMemorySnapshotFromRequest(Resources.GetRepositoryResponse(owner, RepositoryResponseGeneration.July));
            ITableSnapshot microsoftSnapshotJuly = builder.Build();

            builder = RepositoryListSnapshotBuilder.CreateInMemorySnapshotFromRequest(this.Extractor.GetMetadataAsynch(this.Token, owner).Result);
            ITableSnapshot microsoftSnapshotToday = builder.Build();
            
            TableDiffByLookup differ = new TableDiffByLookup(microsoftSnapshotJuly, microsoftSnapshotToday);


            differ.DifferencesDelegate = (deletedRecord, inserted) =>
            {
                EventType et = EventType.Created;
                IDataRecord template = inserted;
                if ((deletedRecord!=null) && (inserted !=null))
                {
                    et = EventType.Modified;
                    template = inserted;
                }

                if ((deletedRecord != null) && (inserted == null))
                {
                    et = EventType.Deleted;
                    template = deletedRecord;
                    //RepositoryTableSnapshot.CreatedAtFieldName
                }

                RepositoryEvent ev = EventFactory.CreateRepositoryEvent(template, et);
                nbuilder.AddEvent(ev);
            };
            differ.Execute();
            //Assert.Equal(1312, differ.OldSnapshotRecordCount);

            //create Notification
            //Deliver Notification
            
            nbuilder.AddChannels(channelsBuilder);

            List<Notification> toSend = new List<Notification>();
            for (int i = 0; i < 5; i++)
            {
                Notification noti = nbuilder.Build();
                noti.From = new NotificationAddress() { Identifier = "wm@schindler-it.com" };
                noti.To = new NotificationAddress[] { noti.From };
                toSend.Add(noti);
            }

            Postman.DeliverNotification(toSend);


        }
    }
}
