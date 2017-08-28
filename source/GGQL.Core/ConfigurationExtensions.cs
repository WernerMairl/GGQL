using GGQL.Core.Internal;
using GGQL.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

namespace GGQL.Core
{
    public static class ConfigurationExtensions
    {

        internal static readonly string PersistLastSnapshotKey = "PersistLastSnapshot";
        internal static readonly string SmtpHostKey = "SmtpHost";
        internal static readonly string SnapshotDirectoryKey = "SnapshotDirectory";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="repositorylogger">logger injected into the resulting repo!</param>
        /// <param name="logger">logger for the configuration access</param>
        /// <returns></returns>
        public static SnapshotRepository CreateSnapShotRepository(this IConfiguration configuration, ILogger repositorylogger, ILogger logger = null)
        {
            Guard.ArgumentNotNull(configuration, nameof(configuration));
            Guard.ArgumentNotNull(repositorylogger, nameof(repositorylogger));
            if (logger == null)
            {
                logger = NullLogger.Instance; //ensure logger !=null!
            }
            string dir = string.Format("{0}", configuration[SnapshotDirectoryKey]);
            if (string.IsNullOrEmpty(dir))
            {
                throw new InvalidOperationException(string.Format("Value for '{0}' not found inside the Configuration (appsettings)", SnapshotDirectoryKey));
            }
            dir = System.Environment.ExpandEnvironmentVariables(dir);
            logger.LogTrace("appsettings.{1}={0}", dir, SnapshotDirectoryKey);
            return new DirectorySnapshotRepository(dir, repositorylogger);
        }

        public static bool GetPersistLastSnapshotSwitch(this IConfiguration configuration, ILogger logger = null)
        {
            Guard.ArgumentNotNull(configuration, nameof(configuration));

            if (logger == null)
            {
                logger = NullLogger.Instance; //ensure logger !=null!
            }

            bool result = true; //the default is true!
            string v = string.Format("{0}", configuration[PersistLastSnapshotKey]);
            logger.LogTrace("appsettings.{1}={0}", v, PersistLastSnapshotKey);
            if (string.IsNullOrEmpty(v))
            {
                return result; //true as default;
            }
            return (bool.Parse(v));
        }

        public static NotificationChannel[] DefineChannels(this IConfiguration configuration, ILogger logger = null)
        {
            Guard.ArgumentNotNull(configuration, nameof(configuration));

            if (logger == null)
            {
                logger = NullLogger.Instance; //ensure logger !=null!
            }

            List<NotificationChannel> list = new List<NotificationChannel>();
            EMailNotificationChannel eMail = new EMailNotificationChannel();
            eMail.SmtpHost = string.Format("{0}", System.Environment.ExpandEnvironmentVariables(configuration[SmtpHostKey]));
            logger.LogTrace("{1}={0}", eMail.SmtpHost, SmtpHostKey);
            list.Add(eMail);
            return list.ToArray();
        }

        public static string[] GetStringArrayFromConfiguration(this IConfiguration configuration, string key, bool expandEnvironmentVariables, ILogger logger = null) //we can use array instead of iEnumerable because the list should not be endless!
        {
            Guard.ArgumentNotNull(configuration, nameof(configuration));
            Guard.ArgumentNotNullOrEmptyString(key, nameof(key));
            if (logger == null)
            {
                logger = NullLogger.Instance; //ensure logger !=null!
            }

            int counter = 0;
            string keyTemplate = key + ":{0}";
            string value = string.Format("{0}", configuration[string.Format(keyTemplate, counter)]);
            List<string> Items = new List<string>();
            while (!string.IsNullOrEmpty(value))
            {
                logger.LogTrace("{1}={0}", value, key);
                if (expandEnvironmentVariables)
                {
                    value = Environment.ExpandEnvironmentVariables(value);
                }
                Items.Add(value);
                counter += 1;
                value = string.Format("{0}", configuration[string.Format(keyTemplate, counter)]);
            }
            if (Items.Count == 0)
            {
                throw new InvalidOperationException(string.Format("No {0} defined inside Configuration (appsettings.json)", key));
            }
            return Items.ToArray();
        }

    }
}
