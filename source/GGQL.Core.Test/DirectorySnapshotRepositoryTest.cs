using Xunit;
using Xunit.Abstractions;
using System.IO;
using System;

namespace GGQL.Core.Test
{
    public class DirectorySnapshotRepositoryTest: IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Directory.Delete(this.TestFolder, true);
            }
        }
        public ITestOutputHelper Helper { get; private set; }
        public DirectorySnapshotRepositoryTest(ITestOutputHelper helper)
        {
            this.Helper = helper;
            this.TestFolder = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this.TestFolder);
        }

        public string TestFolder { get; private set; }


        [Fact]
        public void Try_On_Empty()
        {
            SnapshotRepository repo = new DirectorySnapshotRepository(this.TestFolder);
            Assert.False(repo.TryPull("holla", System.IO.Path.GetTempFileName()));
        }

        [Fact]
        public void Push_And_Pull()
        {
            SnapshotRepository repo = new DirectorySnapshotRepository(this.TestFolder);
            string testContent = "Hello 123456 0815 4711";
            string testkey = "xxyy";
            using (DisposableFile sourceFile = DisposableFile.GetTempFile())
            {
                using (DisposableFile destFile = DisposableFile.GetTempFile())
                {
                    System.IO.File.WriteAllText(sourceFile.Path, testContent);
                    repo.Push(testkey, sourceFile.Path);
                    Assert.True(repo.TryPull(testkey, destFile.Path));
                    Assert.Equal(testContent, File.ReadAllText(destFile.Path));
                }
            }
        }

    }
}
