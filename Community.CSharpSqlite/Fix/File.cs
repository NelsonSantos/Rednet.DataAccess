using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;

namespace Community.CsharpSqlite
{
    public static class File
    {
        public static FileStream Create(string name)
        {
            return new FileStream(name, true);
        }

        public static FileStream Open(string name, bool isForWrite)
        {
            return new FileStream(name, isForWrite);
        }

        public static void Delete(string name)
        {
            // When Csharp-sqlite attempt to delete the file - it does not close the file stream...
            Stream handler = null;
            if (FileStream.HandleTracker.TryGetValue(name, out handler))
            {
                handler.Dispose();

                FileStream.HandleTracker.Remove(name);
            }

            int retry = 0;
            while (retry < 10)
            {
                try
                {
                    Windows.Storage.StorageFile.GetFileFromPathAsync(name).GetResults().DeleteAsync().GetResults(); //IsolatedStorageIO.Default.DeleteFile(name);
                    return;
                }
                catch (Exception)
                {
                    retry++;
                    //Thread.Sleep(100);
                    continue;
                }
            
            }

            throw new InvalidOperationException("Cannot delete file");
        }

        

        public static bool Exists(string name)
        {
            return Windows.Storage.StorageFile.GetFileFromPathAsync(name).GetResults() != null;
        }
    }
}
