using GGQL.Application;
using GGQL.Core.Internal;
using GGQL.Worker;
using System;
using System.Reflection;

namespace GGQL.IssueWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Console.Title = string.Format("{0} v{1}", Helper.GetAssemblyTitle(thisAssembly), Helper.GetProductVersion(thisAssembly));
            Console.WriteLine(Console.Title);
            Console.WriteLine(Helper.GetAssemblyDescription(thisAssembly));
            int exitCode = ConsoleApplication.Execute<IssueWatcherApplication>(args);
            Environment.Exit(exitCode);
        }
    }
}
