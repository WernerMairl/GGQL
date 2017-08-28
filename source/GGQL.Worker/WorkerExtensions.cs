namespace GGQL.Worker
{
    public static class WorkerExtensions
    {

        public static bool IsCompletedSuccessfully(this System.Threading.Tasks.Task t)
        {
            if (t.IsCompleted == false)
            {
                return false;
            }
            if (t.IsCanceled)
            {
                return false;
            }
            if (t.IsFaulted)
            {
                return false;
            }
            return true;
        }
    }
}
