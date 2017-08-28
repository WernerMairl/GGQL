using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GGQL.Test.Resources
{
    public static class Resources
    {
        public static string DefaultIntrospectionQuery =  @"{
  __schema {
    types {name}
  }
}";

        internal static Assembly ThisAssembly = typeof(Resources).Assembly;

        public static string[] GetResourceNames()
        {
            return ThisAssembly.GetManifestResourceNames();
        }

        public static string FBEventsSample = @"";
        private static string GetAsString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            using (Stream stream = ThisAssembly.GetManifestResourceStream(key))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                     return sr.ReadToEnd();
                }
            }
        }
        private static IEnumerable<string> GetZipEntriesAsStrings(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            using (Stream stream = ThisAssembly.GetManifestResourceStream(key))
            {
                using (System.IO.Compression.ZipArchive za = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read, leaveOpen: false))
                {
                    foreach (System.IO.Compression.ZipArchiveEntry ze in za.Entries)
                    {
                        using (Stream zs = ze.Open())
                        {
                            using (StreamReader sr = new StreamReader(zs))
                            {
                                yield return sr.ReadToEnd();
                            }

                        }
                    }
                }


            }
        }

        public static string GetTRVEvents()
        {
            string key = string.Format("GGQL.Test.Resources.FBEventRequests.TRVEvents.json");
            return GetAsString(key);
        }


        public static string EventsOverCursorFirstResponse()
        {
            return GetAsString("GGQL.Test.Resources.FBEventRequests.CursorFirstResponse.json");
        }


        public static string EventsOverCursorLastResponse()
        {
            return GetAsString("GGQL.Test.Resources.FBEventRequests.CursorLastResponse.json");
        }


        public static string EventsOverCursorMiddleResponse()
        {
            return GetAsString("GGQL.Test.Resources.FBEventRequests.CursorMiddleResponse.json");
        }


        public static string GetTwoEventNodes()
        {
            string key = string.Format("GGQL.Test.Resources.FBEventRequests.TwoEventNodes.json");
            return GetAsString(key);
        }



        public static string GetIssuesResponse_FirstDraft()
        {
            string key = string.Format("GGQL.Test.Resources.IssueRequests.FirstDraft.json");
            return GetAsString(key);
        }

        public static IEnumerable<string> GetRepositoryResponse(string repositoryOwner, RepositoryResponseGeneration generation)
        {
            if (string.IsNullOrEmpty(repositoryOwner))
            {
                throw new ArgumentNullException(nameof(repositoryOwner));
            }
            string gen = generation.ToString();
            string key = string.Format("GGQL.Test.Resources.RepositoryRequests.{0}_{1}.zip", repositoryOwner, gen);
            return GetZipEntriesAsStrings(key);
        }





    }
}
