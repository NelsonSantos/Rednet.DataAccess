using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rednet.Test.Console
{
    public static class SharedFunctions
    {

        public static Image ByteArrayToImage(byte[] value)
        {
            if (value != null)
            {
                var _ms = new MemoryStream(value, 0, value.Length);
                _ms.Position = 0; // this is important
                return Image.FromStream(_ms, true);
            }
            return null;
        }

        private static string CurrentAppPath
        {
            get
            {
                var _codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var _uri = new UriBuilder(_codeBase);
                var _path = Uri.UnescapeDataString(_uri.Path);
                return Path.GetDirectoryName(_path);
            }
        }

        public static string GetUploadedFilesDir()
        {
            var _dir = Path.Combine(CurrentAppPath, "Uploads");

            if (!Directory.Exists(_dir))
                Directory.CreateDirectory(_dir);

            return _dir;
        }

        public static string GetDatabaseDir()
        {
            var _dir = Path.Combine(CurrentAppPath, "Database");

            if (!Directory.Exists(_dir))
                Directory.CreateDirectory(_dir);

            return _dir;
        }

        public static string GetScriptsDir()
        {
            var _dir = Path.Combine(CurrentAppPath, "Scripts");

            if (!Directory.Exists(_dir))
                Directory.CreateDirectory(_dir);

            return _dir;
        }

        public static string GetDatabaseFile()
        {
            var _file = Path.Combine(GetDatabaseDir(), "database.db3");

            if (File.Exists(_file)) return _file;

            using (var _fs = File.Create(_file))
            {
                _fs.Close();
            }

            return _file;
        }

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(GetDatabaseFile());
        }

        public static void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Atenção...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Atenção...", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Atenção...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult ShowQuestionMessage(string message)
        {
            return MessageBox.Show(message, "Atenção...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
