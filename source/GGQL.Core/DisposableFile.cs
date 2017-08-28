using System;
using System.IO;

namespace GGQL.Core
{

    public sealed class DisposableFile :  IDisposable
    {
        public static DisposableFile GetTempFile()
        {
            return new DisposableFile(System.IO.Path.GetTempFileName());
        }
        public static DisposableFile CreateFromStream(string path, Stream data)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            using (FileStream fs = File.Create(path))
            {
                data.CopyTo(fs);
            }
            return new DisposableFile(path);
        }

        public string Path { get; private set; }
        public DisposableFile(string path)
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
                if (Path != null && File.Exists(Path))
                {
                    try
                    {
                        File.Delete(Path);
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
