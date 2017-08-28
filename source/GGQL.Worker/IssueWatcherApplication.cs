using GGQL.Core;
using GGQL.Core.Internal;
using Microsoft.Extensions.Logging;
using SQLiteExtensions;
using System.Collections.Generic;

namespace GGQL.Worker
{
    public class IssueWatcherApplication : WorkerApplication
    {

        public static IEnumerable<string> ExtractIssuesByRepositoryPath(string token, string repositoryPath)
        {
            GithubRepositoryMetadataExtractor Extractor = new GithubRepositoryMetadataExtractor();
            string[] splits = repositoryPath.Split('/');
            return Extractor.GetIssueMetadataAsynch(token, owner: splits[0], repositoryName: splits[1]).Result;
        }


        public override int ExecuteAndReturn()
        {
            Guard.AssertNotNull(this.CurrentWork);
            //return Handle_GithubRepositoriesWithIssues<IssueEvent>(this.CurrentWork, this.PersistLastSnapshot, base.Logger);
            return HandleWork<IssueEvent>(this.Console, this.CurrentWork, this.Logger, (w, i) =>
            {
                string key = w.GetKey(i);

                return CreateEvents(key, (sf) => 
                {
                    return IssueListSnapshotBuilder.CreateFileSnapshotFromRequest(sf, ExtractIssuesByRepositoryPath(w.AccessToken, i));
                }
                , (sf2) =>
                {
                    bool exists = w.Repository.TryPull(key, sf2);
                    this.Logger.LogTrace("GetLastSnapshot.SnapshotFilename={0} (Exists={1})", key, exists);
                    if (exists)
                    {
                        ITableSnapshotBuilder builder = IssueListSnapshotBuilder.CreateFromFile(sf2);
                        return builder;
                    }
                    else
                    {
                        return IssueListSnapshotBuilder.EmptySnapshot();
                    }
                }
                , w.Repository, this.PersistLastSnapshot, this.Logger, (dr, et)
                    => EventFactory.CreateIssueEvent(dr, et)
                );
            });
        }





        protected override int Initialize()
        {
            Guard.AssertNull(this.CurrentWork);
            WorkerInitialization(WorkItemType.Issue, "Repositories"); //"Repositories" is the name of the Json Item inside appsettings.json!
            Guard.AssertNotNull(this.CurrentWork);
            return 0;
        }
    }
}
