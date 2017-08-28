using GGQL.Core;
using GGQL.Core.Internal;
using Microsoft.Extensions.Logging;
using SQLiteExtensions;
using System.Collections.Generic;

namespace GGQL.Worker
{
    public class RepositoryWatcherApplication : WorkerApplication
    {

        public static IEnumerable<string> ExtractRepositoriesByOwner(string token, string owner)
        {
            GithubRepositoryMetadataExtractor Extractor = new GithubRepositoryMetadataExtractor();
            return Extractor.GetMetadataAsynch(token, owner).Result;
        }


        public override int ExecuteAndReturn()
        {
            Guard.AssertNotNull(this.CurrentWork);
            return HandleWork<RepositoryEvent>(this.Console, this.CurrentWork, this.Logger, (w, i) =>
            {
                string key = w.GetKey(i);

                return CreateEvents(key, (sf) =>
                {
                    return RepositoryListSnapshotBuilder.CreateFileSnapshotFromRequest(sf, ExtractRepositoriesByOwner(w.AccessToken, i));
                }
                , (sf2) =>
                {
                    bool exists = w.Repository.TryPull(key, sf2);
                    this.Logger.LogTrace("GetLastSnapshot.SnapshotFilename={0} (Exists={1})", key, exists);
                    if (exists)
                    {
                        ITableSnapshotBuilder builder = RepositoryListSnapshotBuilder.CreateFromFile(sf2);
                        return builder;
                    }
                    else
                    {
                        return RepositoryListSnapshotBuilder.EmptySnapshot();
                    }
                }
                , w.Repository, this.PersistLastSnapshot, this.Logger, (dr, et)
                    => EventFactory.CreateRepositoryEvent(dr, et)
                );
            });
        }

        protected override int Initialize()
        {
            Guard.AssertNull(this.CurrentWork);
            WorkerInitialization(WorkItemType.Repository, "RepositoryOwners"); //"RepositoryOwners" is the name of the Json Item inside appsettings.json!
            Guard.AssertNotNull(this.CurrentWork);
            return 0;
        }



    }
}
