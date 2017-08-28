using System;
using System.IO;

namespace GGQL.Core
{
    public sealed class DisposableDirectory : IDisposable
    {
        public static DisposableDirectory GetTempDirectory()
        {
            string s = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(s);
            return new DisposableDirectory(s);
        }

        public string Path { get; private set; }
        public DisposableDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            this.Path = path;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Path != null && Directory.Exists(Path))
                {
                    try
                    {
                        Directory.Delete(Path,recursive:true);
                    }
                    catch
                    {
                        //DO NOTHING!
                    }
                }
            }
        }




    }
}
