using System.Collections.Generic;

namespace Rednet.DataAccess
{
    public static class DatabaseObjectShared
    {
        private static IDictionary<string, string[]> m_PrimaryKeys = new Dictionary<string, string[]>();
        private static Dictionary<string, IDataFunctions> m_DataFunction = new Dictionary<string, IDataFunctions>();

        public static DatabaseType CurrentDatabaseType { get; set; }

        public static Dictionary<string, IDataFunctions> DataFunction
        {
            get { return m_DataFunction; }
            set { m_DataFunction = value; }
        }

        public static IDictionary<string, string[]> PrimaryKeys
        {
            get { return m_PrimaryKeys; }
            set { m_PrimaryKeys = value; }
        }

        public static string DatabaseName { get; set; }
    }
}