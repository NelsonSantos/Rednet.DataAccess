using System.Collections.Generic;
#if NET45
using System.Web;
#endif

namespace Rednet.DataAccess
{
    public static class DatabaseObjectShared
    {

        private static IDictionary<string, string[]> m_PrimaryKeys = new Dictionary<string, string[]>();
        private static Dictionary<string, IDataFunctions> m_DataFunction = new Dictionary<string, IDataFunctions>();
        private static bool m_UseHttpContextCurrentSessionForStore = false;

        public static DatabaseType CurrentDatabaseType { get; set; }

        public static Dictionary<string, IDataFunctions> DataFunctions
        {
            get
            {
#if NET45
                if (!UseHttpContextCurrentSessionForStore) return m_DataFunction;

                var _ret = (Dictionary<string, IDataFunctions>) HttpContext.Current.Session["datafunction"];
                if (_ret == null)
                {
                    _ret = new Dictionary<string, IDataFunctions>();
                    HttpContext.Current.Session["datafunction"] = _ret;
                }
                return _ret;
#else
                return m_DataFunction;
#endif
            }
            //set { m_DataFunction = value; }
        }

        public static IDictionary<string, string[]> PrimaryKeys
        {
            get
            {
#if NET45
                if (!UseHttpContextCurrentSessionForStore) return m_PrimaryKeys;

                var _ret = (Dictionary<string, string[]>)HttpContext.Current.Session["primarykeys"];
                if (_ret == null)
                {
                    _ret = new Dictionary<string, string[]>();
                    HttpContext.Current.Session["primarykeys"] = _ret;
                }
                return _ret;
#else
                return m_PrimaryKeys;
#endif
            }
            //set { m_PrimaryKeys = value; }
        }

        public static string DefaultDataFunctionName { get; set; }

        public static bool UseHttpContextCurrentSessionForStore
        {
            get { return m_UseHttpContextCurrentSessionForStore; }
            set { m_UseHttpContextCurrentSessionForStore = value; }
        }
    }
}