using System;
using System.IO;
using System.Text;

namespace Community.CsharpSqlite
{
    public static class Path
    {
        public static string GetTempPath()
        {
            return string.Empty;
        }

        public static string GetTempFileName()
        {
            return Guid.NewGuid().ToString();
        }

        public static string Combine( string path1, string path2 )
        {
            return System.IO.Path.Combine(path1, path2);
        }

        public static string GetFullPath(string path)
        {
            // try removing ./ and ../
            StringBuilder sb = new StringBuilder(path);
            while ( true )
            {
                if (sb[0] == '.' && sb[1] == '/')
                {
                    sb.Remove(0, 2);
                    continue;
                }

                if (sb[0] == '.' && sb[1] == '.' && sb[2] == '/')
                {
                    sb.Remove(0, 3);
                    continue;
                }

                return sb.ToString();
            }
        }
    }
}
