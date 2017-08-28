namespace GGQL.Core
{
    public abstract class SnapshotRepository
    {
        //We have multiple SnapshotRepositories, we must separate them or the content
        //- by WorkItem
        //- by User and Monitor
        //- by Owner/Repo/Issue

        public abstract bool TryPull(string key, string localFilePath);
        public abstract void Push(string key, string localFilePath);
    }

}
