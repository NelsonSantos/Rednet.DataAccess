using System;
using System.Net;
using System.Collections;
using Community.CsharpSqlite.SQLiteClient;
using System.Collections.Generic;
using System.Data;

namespace Community.CsharpSqlite.SQLiteClient
{
    /// <summary>
    /// Class which make sure that there is only one active query on the database
    /// </summary>
    public static class SqliteConnectionManager
    {
        private static Dictionary<string, SqliteConnection> _Connection = new Dictionary<string, SqliteConnection>();

        /// <summary>
        /// Execute command on the database, commands are serialized
        /// </summary>
        /// <param name="database"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static IEnumerable<IDataReader> ExecuteCommand(string database, string commandText)
        {
            SqliteConnection conn;
            if (!_Connection.TryGetValue(database, out conn))
            {
                conn = new SqliteConnection(SqliteConnectionManager.GetConnectionString(database));
                conn.Open();
                _Connection.Add(database, conn);
            }

            lock (conn)
            {
                var command = conn.CreateCommand();
                command.CommandText = commandText;

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        yield return result;
                    }
                }

                command.Dispose();
            }
        }

        public static void CloseConnection(string database)
        {
            SqliteConnection conn;
            if (_Connection.TryGetValue(database, out conn))
            {
                conn.Close();
                conn.Dispose();
            }
        }

        private static string GetConnectionString( string dbFile )
        {
            return string.Format("Version=3,uri=file:{0}", dbFile);
        }
    }
}
