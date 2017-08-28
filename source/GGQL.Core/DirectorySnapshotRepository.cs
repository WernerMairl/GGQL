using System;
using System.IO;
using GGQL.Core.Internal;
using Microsoft.Extensions.Logging;
namespace GGQL.Core
{
    public class DirectorySnapshotRepository : SnapshotRepository
    {
        private static readonly string Extension = "sqlite";
        internal ILogger Logger { get; private set; }
        public string StorageDirectory { get; private set; }

        public DirectorySnapshotRepository(string directory) : this(directory, Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance)
        { }

        public DirectorySnapshotRepository(string directory, ILogger logger)
        {
            Guard.ArgumentNotNullOrEmptyString(directory, nameof(directory));
            Guard.ArgumentNotNull(logger, nameof(logger));
            this.Logger = logger;
            if (Directory.Exists(directory) == false)
            {
                logger.LogTrace("Directory {0} does not exists and is created", directory);
                Directory.CreateDirectory(directory);
            }
            this.StorageDirectory = directory;
            logger.LogTrace("DirectorySnapshotRepository={0}", directory);
        }

        private string CalculateFilename(string key, string extension)
        {
            Guard.AssertNotNullOrEmptyString(key);
            Guard.AssertNotNullOrEmptyString(extension);
            Guard.Assert(extension.StartsWith(".") == false);
            Guard.Assert(key == key.Trim(), "trim111");
            foreach (char ic in Path.GetInvalidFileNameChars())
            {
                if (key.Contains(ic.ToString()))
                {
                    throw new InvalidOperationException(string.Format("'{1}' not alled in key strings ('{0}')",key,ic));
                }
            }

            string fn = string.Format("{1}.latest.{0}", extension, key);
            return Path.Combine(this.StorageDirectory, fn);
        }

        public override bool TryPull(string key, string localFilePath)
        {
            Guard.ArgumentNotNullOrEmptyString(key, nameof(key));
            Guard.ArgumentNotNullOrEmptyString(localFilePath, nameof(localFilePath));
            string neededFile = CalculateFilename(key, DirectorySnapshotRepository.Extension);
            if (File.Exists(neededFile) == false)
            {
                this.Logger.LogTrace("TryPull==false, '{0}' does not exists", neededFile);
                return false;
            }
            File.Copy(neededFile, localFilePath, overwrite: true);
            this.Logger.LogTrace("TryPull==true, '{0}' provided as '{1}'", neededFile,localFilePath);
            return true;
        }

        public override void Push(string key, string localFilePath)
        {
            Guard.ArgumentNotNullOrEmptyString(key, nameof(key));
            Guard.ArgumentNotNullOrEmptyString(localFilePath, nameof(localFilePath));
            string dropFile = CalculateFilename(key, DirectorySnapshotRepository.Extension);
            File.Copy(localFilePath,dropFile, overwrite: true);
            this.Logger.LogTrace("Push from '{0}' to '{1}'", localFilePath,dropFile);
        }
    }

}
