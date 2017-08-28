using GGQL.Core.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Console.Internal;
using System;
using System.Runtime.InteropServices;

namespace GGQL.Application
{

    //Avoid Disposable for this class, it makes the situation more complex specially shutdon in case of error (ExceptionHandler-Method with "isDisposed" ??

    public abstract class ConsoleApplication 
    {
        public LoggerFactory LoggerFactory { get; private set; }

        internal int OnIntialize(LoggerFactory loggerFactory, string[] args)
        {
            Guard.AssertNotNull(loggerFactory);
            this.LoggerFactory = loggerFactory;
            int i = InitializeCore(args);
            if (i != 0)
            { 
                return i;
            }
            Guard.AssertNotNull(this.LoggerFactory);
            if (this.Logger == this.FallbackLogger)
            {
                this.Logger = this.LoggerFactory.CreateLogger(this.GetType()); //use loggerfacory
            }
            return Initialize();
        }
        protected abstract int InitializeCore(string[] args);

        protected abstract int Initialize(); //use abstract instead of virtual to avoid ANY dependency to base.Initialize()
        public abstract int ExecuteAndReturn();

        public static int FallbackExceptionHandler(Exception exception, IConsole console)
        {
            if (exception != null)
            {
                if (console != null)
                {
                    console.WriteLine(exception.Message, null, ConsoleColor.Red);
                    if (string.IsNullOrEmpty(exception.Source) == false)
                    {
                        //console.WriteLine("",null,null);
                        console.WriteLine(string.Format("Source: {0}",exception.Source), null, ConsoleColor.Red);
                    }
                }
            }
            return 99;
        }
        public static int Execute<T>(string[] args) where T : ConsoleApplication
        {

            //1. instantiate Application.
            //if a exception is thrown during constructor call, there is no Application specific Exception handler available (defined inside the Application)
            //in this case, use the FallbackExceptionHandler!
            ConsoleApplication app = null;
            try
            {

                app = System.Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                return FallbackExceptionHandler(ex,null);
            }
            Guard.AssertNotNull(app);
            try
            {
                LoggerFactory loggerFactory = new LoggerFactory();
                
                int parserResult = app.OnIntialize(loggerFactory, args);
                Guard.Assert(app.LoggerFactory == loggerFactory, "should be set for finally");

                if (parserResult != 0)
                {
                    return parserResult;
                }
                return app.ExecuteAndReturn();
            }
            catch (Exception ex)
            {
                int returncode;
                if (app.ExceptionHandler == null)
                {
                    returncode = FallbackExceptionHandler(ex, app.Console);
                    if (app.Logger != null)
                    {
                        app.Logger.LogDebug(ex, "", null);
                    }
                }
                else
                {
                    returncode = app.ExceptionHandler(ex);
                }
                if (app.Logger!=null)
                {
                    app.Logger.LogTrace("ExitCode={0}", returncode);
                }
                return returncode;
            }
            finally
            {
                if (app.LoggerFactory != null)
                {
                    app.LoggerFactory.Dispose(); //implicites flush !! YES and really important!
                }
            }
        }

        private static readonly string LoggerName = "ConsoleApplication";
        private ILogger FallbackLogger;
        protected bool ShowTraceMessages { get; set; }
        /// <summary>
        /// Argument must be injected by Constructor because the entire behavior, including Exception Handling, Output Handling, Configuration Handling can be depending on Arguments
        /// </summary>
        public ConsoleApplication()
        {
            this.ShowTraceMessages = false; //the default. can/should be overwritten in descendand classes.Initialize!
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.Console = new WindowsLogConsole();
            }
            else
            {
                this.Console = new AnsiLogConsole(new AnsiSystemConsole());
            }
            //we overwrite this later by using Loggerfactory, but we should initialize! 
            this.Logger = new ConsoleLogger(LoggerName, (ln, logLevel) => 
            {
                if (ShowTraceMessages == false)
                {
                    return false;
                }
                if (ln!=LoggerName)
                {
                    return false;
                }
                return (logLevel >= LogLevel.Information); 
            }, includeScopes: false);
            this.FallbackLogger = this.Logger;
        }

        /// <summary>
        /// resulttype int because Exitcode(int) is the most important result on Console App
        /// </summary>
        public System.Func<System.Exception, int> ExceptionHandler { get; set; }

        public IConsole Console { get; private set; }

        public ILogger Logger { get; protected set; }


    }
}
