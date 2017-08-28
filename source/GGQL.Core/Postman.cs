using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Collections.Concurrent;
using GGQL.Core.Internal;
using System.Collections;
using System;
using System.Threading.Tasks;
using GGQL.Model;

namespace GGQL.Core
{

    public class Postman
    {


        private static Task CreateChannelTask(NotificationChannel channel, IEnumerable<Notification> workItems)
        {
            return Task.Factory.StartNew(() =>
            {
                channel.SendNotification(workItems);
            });
        }

        public static void DeliverNotification(IEnumerable<Notification> notifications)
        {
            Guard.ArgumentNotNull(notifications, nameof(notifications));
            //Problem: we need the Notifications grouped by Channel, because each channel needs a IEnumerable to iterate over by using only ONE Connection to the channel implementer
            //Idea: use one TASK per Channel
            Dictionary<NotificationChannel, Tuple<BlockingCollection<Notification>, Task>> grouping = new Dictionary<NotificationChannel, Tuple<BlockingCollection<Notification>, Task>>();

            try

            {
                foreach (Notification n in notifications)
                {
                    foreach (NotificationChannel ch in n.Channels)
                    {
                        Tuple<BlockingCollection<Notification>, System.Threading.Tasks.Task> tuple;
                        if (grouping.TryGetValue(ch, out tuple) == false)
                        {
                            BlockingCollection<Notification> group = new BlockingCollection<Notification>();
                            Task t = CreateChannelTask(ch, group.GetConsumingEnumerable());
                            tuple = new Tuple<BlockingCollection<Notification>, System.Threading.Tasks.Task>(group, t);
                            grouping.Add(ch, tuple);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("PING");
                        }
                        Guard.AssertNotNull(tuple);
                        Guard.AssertNotNull(tuple.Item1);
                        Guard.AssertNotNull(tuple.Item2);
                        tuple.Item1.Add(n);
                    }
                }

            }
            finally
            {
                List<Task> waiters = new List<Task>();
                foreach (var t in grouping.Values)
                {
                    t.Item1.CompleteAdding();
                    waiters.Add(t.Item2);
                }
                Task.WaitAll(waiters.ToArray());
            }
        }
    }

}
