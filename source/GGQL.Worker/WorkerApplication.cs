using GGQL.Application;
using GGQL.Core;
using GGQL.Core.Internal;
using GGQL.Model;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console.Internal;
using SQLiteExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace GGQL.Worker
{
    /// <summary>
    /// Abstract Application class for all the GGQL Worker applications (RepositoryWatcher, IssueWatcher....) 
    /// shared code for GitHub access and other shareble features!
    /// </summary>
    public abstract class WorkerApplication : ConsoleApplication
    {
        private static string Pluralize(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            s = s.TrimEnd();
            if (s.EndsWith("y"))
            {
                return s.Substring(0, s.Length - 1) + "ies"; //Repository =ies
            }
            return s+ "s"; //Issue+s
        }
        protected void WorkerInitialization(WorkItemType itemType, string itemsDefinitionKey)
        {
            Guard.AssertNotNullOrEmptyString(itemsDefinitionKey);
            Guard.AssertNotNull(this.SnapshotRepository);

            WorkDescription w = new WorkDescription(this.SnapshotRepository, itemType);
            w.AccessToken = this.Configuration[WorkerApplication.TokenVariableName];
            if (string.IsNullOrEmpty(w.AccessToken))
            {
                throw new InvalidOperationException(string.Format("{0} not available via appsettings or environment variable", TokenVariableName));
            }

            w.Items = this.Configuration.GetStringArrayFromConfiguration(itemsDefinitionKey,false,this.Logger);

            w.Channels = this.Configuration.DefineChannels(this.Logger);

            string fromEmail = string.Format("{0}", this.Configuration["eMail.From"]);
            if (string.IsNullOrEmpty(fromEmail))
            {
                throw new InvalidOperationException("eMail.From not defined in appsettings");
            }
            fromEmail = System.Environment.ExpandEnvironmentVariables(fromEmail);
            if (string.IsNullOrEmpty(fromEmail))
            {
                throw new InvalidOperationException("eMail.From empty afer ExpandEnvironmentVariables");
            }


            NotificationAddress fromAddress = new NotificationAddress() { Identifier = fromEmail };
            base.Logger.LogTrace("eMail.From={0}", fromEmail);

            string[] to = this.Configuration.GetStringArrayFromConfiguration("eMail.To",true, this.Logger);
            List<NotificationAddress> toAddresses = new List<NotificationAddress>();
            foreach (string value in to)
            {
                toAddresses.Add(new NotificationAddress() { Identifier = value });
            }

            w.From = fromAddress;
            w.To = toAddresses.ToArray();
            this.CurrentWork = w;
        }

        protected bool PersistLastSnapshot { get; private set; } //false only for testing environments!


        public static readonly string TokenVariableName = "GithubToken";

        public WorkDescription CurrentWork { get; protected set; }
        public CommandOption CmdVerbose { get; private set; }

        protected virtual void InitializeCommandLineParser(CommandLineApplication application)
        { }

        protected override int InitializeCore(string[] args)
        {
            Guard.Assert(this.ShowTraceMessages == false, "the expected default");
            LogLevel minLogLevel = LogLevel.None;

            foreach (string a0 in args)
            {
                if (string.IsNullOrEmpty(a0)) continue;
                if (a0.ToLowerInvariant() == "-v") this.ShowTraceMessages = true;
                if (a0.ToLowerInvariant() == "--verbose") this.ShowTraceMessages = true;
            }
            if (ShowTraceMessages)
            {
                minLogLevel = LogLevel.Information; //we use Information for -Verbose, WM 21.08.2017
            }
            //argument pre-parser for trace/debug/console --preparse because the full parser should use logger/tracing!


            Guard.AssertNotNull(this.LoggerFactory);
            bool appsettingsRequired = true; //currently not able to work without appsettings, no defaults available for all the options!
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            string configFile = "appsettings.json";
#if DEBUG
            base.Logger.LogTrace("#IF DEBUG found (assuming we run under VS/Dev");
            //Requirements: some sensitive information can be inside appsettings.json. 
            // - this information should NOT be inside git/svn
            // if the developer is working with different branches probably he needs always the same settings to work with his local environment
            builder = builder.AddJsonFile(configFile, optional: !appsettingsRequired);

#else
            builder = builder.AddJsonFile(configFile, optional: !appsettingsRequired);
#endif
            IConfiguration root = builder.Build();

            //AddEnvironment
            string s = System.Environment.GetEnvironmentVariable(TokenVariableName);
            if (string.IsNullOrEmpty(s) == false)
            {
                root[TokenVariableName] = s;
            }
            this.Configuration = root;
            CommandLineApplication CmdApp = new CommandLineApplication(throwOnUnexpectedArg: false);
            //CommandOption help = cmdApp.Option("-?|-h|--help", "Help", CommandOptionType.NoValue);
            this.CmdVerbose = CmdApp.Option("-v|--verbose", "Verbose", CommandOptionType.NoValue);
            InitializeCommandLineParser(CmdApp);
            CmdApp.Execute(args);

            Guard.Assert(CmdVerbose.HasValue() == this.ShowTraceMessages);

            this.LoggerFactory.AddConsole(minLogLevel, includeScopes: true); //Trace=0, Debug=1, Information =2, Warning = 3

            if (CmdApp.RemainingArguments.Count > 0)
            {
                this.Console.WriteLine(CmdApp.GetHelpText(), null, null);
                return 1;
            }

            this.SnapshotRepository = this.Configuration.CreateSnapShotRepository(this.Logger,this.Logger);
            this.PersistLastSnapshot = this.Configuration.GetPersistLastSnapshotSwitch(this.Logger); //default behavior is true, false only for test scenarios!

            return 0;
        }
        protected SnapshotRepository SnapshotRepository { get; private set; }
        public IConfiguration Configuration { get; protected set; }



        public static int HandleWork<T>(IConsole console, WorkDescription currentWork, ILogger logger, Func<WorkDescription, string, IEvent[]> callback) where T : IEvent
        {
            Guard.ArgumentNotNull(currentWork, nameof(currentWork));
            Guard.ArgumentNotNull(logger, nameof(logger));
            Guard.ArgumentNotNull(currentWork.Channels, "currentWork.Channels");

            List<IEvent> FullList = new List<IEvent>();
            List<Task<Tuple<IEvent[], TimeSpan>>> TaskList = new List<Task<Tuple<IEvent[], TimeSpan>>>();
            List<Task> continuations = new List<Task>();

            foreach (string item in currentWork.Items)
            {
                lock (console)
                {
                    console.WriteLine(string.Format("  {0} Task started...", item), null, null);
                }

                Task<Tuple<IEvent[], TimeSpan>> task = Task.Factory.StartNew<Tuple<IEvent[], TimeSpan>>( () =>
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    IEvent[] ev= callback(currentWork, item);
                    sw.Stop();
                    return new Tuple<IEvent[], TimeSpan>(ev, sw.Elapsed);
                });

                Task ct = task.ContinueWith( (prec) =>
                {
                    lock (console)
                    {
                        console.WriteLine(string.Format("  {0} Task completed",item), null, null);
                        console.WriteLine(string.Format("    {0} Events identified in {1}ms", prec.Result.Item1.Length, Convert.ToInt64(prec.Result.Item2.TotalMilliseconds)), null, null);
                    }
                }, TaskContinuationOptions.NotOnFaulted);
                TaskList.Add(task);
                continuations.Add(ct);
            }

            Task.WaitAll(TaskList.ToArray());
            Task.WaitAll(continuations.ToArray());

            NotificationBuilder nbuilder = new NotificationBuilder().UseHtmlRenderer();

            //Date Hierarchy (Year/Month/Day) created by Blogger, we must only provide a meaningfull subject

            string itemsAsString = string.Join(",", currentWork.Items);
            nbuilder.UseSubject(string.Format("{0} {1}", itemsAsString, Pluralize(currentWork.ItemType.ToString())));

            foreach (Task<Tuple<IEvent[], TimeSpan>> t in TaskList)
            {
                nbuilder.AddEvents(t.Result.Item1);
            }

            if (nbuilder.HasEvents == false)
            {
                console.WriteLine("Now changes found", null, null);
                return 0; //NO NOTIFICATION required!
            }
            nbuilder.AddChannels(currentWork.Channels);
            Notification noti = nbuilder.Build();
            noti.From = currentWork.From;
            noti.To = currentWork.To;
            
            Postman.DeliverNotification(new Notification[] { noti });
            console.WriteLine("", null, null);
            console.WriteLine(string.Format("{0} changes delivered", nbuilder.EventCount), null, null);
            return 0;
        }

        public static IEvent[] CreateEvents(string key, Func<string, ITableSnapshotBuilder> currentQuerySnapshotBuilderDelegate, Func<string, ITableSnapshotBuilder> lastSnapshotBuilderDelegate, SnapshotRepository snapshotRepository, bool persistAsLastSnapshot, ILogger logger, Func<IDataRecord, EventType, IEvent> factoryCallback)
        {
            Guard.ArgumentNotNullOrEmptyString(key, nameof(key));
            Guard.ArgumentNotNull(currentQuerySnapshotBuilderDelegate, nameof(currentQuerySnapshotBuilderDelegate));
            Guard.ArgumentNotNull(lastSnapshotBuilderDelegate, nameof(lastSnapshotBuilderDelegate));
            Guard.AssertNotNull(logger);
            Guard.AssertNotNull(snapshotRepository);
            List<IEvent> events = new List<IEvent>();

            using (DisposableFile temporarySnapshot = DisposableFile.GetTempFile())
            {
                Task<ITableSnapshot> queryNow = Task.Factory.StartNew<ITableSnapshot>(() =>
                {
                    ITableSnapshotBuilder builder = currentQuerySnapshotBuilderDelegate(temporarySnapshot.Path);
                    ITableSnapshot snapShotFromRequest = builder.Build();
                    return snapShotFromRequest;
                });

                Task<ITableSnapshot> lastSnapshot1 = Task.Factory.StartNew<ITableSnapshot>(() =>
                {
                    string localTempFile = Path.GetTempFileName(); //DELETE The File at the end ??
                    ITableSnapshotBuilder builder = lastSnapshotBuilderDelegate(localTempFile);
                    ITableSnapshot snapShotFromRequest = builder.Build();
                    return snapShotFromRequest;
                });


                int msTimeoutLoggingSteps = 1000;
                while (Task.WaitAll(new Task[] { queryNow, lastSnapshot1 }, msTimeoutLoggingSteps) == false)
                {
                    logger.LogTrace("CreateEvents.Wait-Loop.{0}ms for '{1}'", msTimeoutLoggingSteps, key);
                }


                try
                {
                    TableDiffByLookup differ = new TableDiffByLookup(lastSnapshot1.Result, queryNow.Result);
                    differ.DifferencesDelegate = (deletedRecord, inserted) =>
                    {
                        EventType et = EventType.Created;
                        IDataRecord template = inserted;
                        if ((deletedRecord != null) && (inserted != null))
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
                        if (et != EventType.Modified)
                        {
                            //IEvent ev = //EventFactory.CreateRepositoryEvent(template, et);
                            IEvent ev = factoryCallback(template, et);
                            Guard.AssertNotNull(ev);
                            events.Add(ev);
                        }
                        else
                        {
                            // we ignore modifications!
                        }
                    };
                    differ.Execute();
                }
                finally
                {
                    if (queryNow.IsCompletedSuccessfully())
                    {
                        queryNow.Result.Dispose();
                    }
                    if (lastSnapshot1.IsCompletedSuccessfully())
                    {
                        lastSnapshot1.Result.Dispose();
                    }
                }
                if (persistAsLastSnapshot)
                {
                    snapshotRepository.Push(key, temporarySnapshot.Path);
                }
                else
                {
                    logger.LogInformation("Snapshot {0} NOT persited!", key);
                }

            } //using temporary snapshotfile disposable

            return events.ToArray();
            //throw new NotImplementedException();
        }


    }
}
