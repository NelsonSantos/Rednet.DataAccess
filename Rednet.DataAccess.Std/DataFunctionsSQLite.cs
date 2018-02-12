using System.Linq;
using System.Collections.Generic;
using Rednet.DataAccess.Dapper;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Rednet.DataAccess
{
    public class DataFunctionsSQLite : DataFunctions<DataFunctionsSQLite>
    {

        private static IDbConnection m_Connection = null;

        public DataFunctionsSQLite(string databaseFile)
            : this()
        {
            this.DatabaseFile = databaseFile;
            this.GetNewConnection();
        }

        public DataFunctionsSQLite()
            : base(DatabaseType.SQLite)
        {
            this.GetNewConnection();
        }

        public string DatabaseFile { get; set; }

        public override IDbConnection Connection
        {
            get
            {
                lock (m_Connection)
                {
                    this.GetNewConnection();
                    return m_Connection;
                }
            }
        }

        private void GetNewConnection()
        {
            m_Connection = new SqliteConnection(this.GetConnectionString());
        }

        public override string GetConnectionString()
        {
            return string.Format("Data Source={0};", this.DatabaseFile, this.Pooling);
        }

        public override string GetSqlLastIdentity()
        {
            return "; select last_insert_rowid() as ultid";
        }

        public override string GetDateTimeFormat()
        {
            return "strftime('%Y-%m-%d %H:%M:%S', '{0}')";
        }

        protected override bool CheckTableExists(string tableName)
        {
#if !PCL
            using (var _conn = this.Connection)
            {
                return _conn.Query<TableInfo>(string.Format("select * from sqlite_master where type='table' and lower(name)='{0}'", tableName.ToLower())).Any();
            }
#else
            return base.CheckTableExists(tableName);
#endif
        }

        public override void RenameTable(string fromName, string toName)
        {
#if !PCL
            using (var _conn = this.Connection)
            {
                _conn.Execute(string.Format("  alter table {0} rename to {1}", fromName, toName));
            }
#endif
        }

        public override List<DbColumnDef> GetColumnsDef(string tableName)
        {
#if !PCL
            using (var _conn = this.Connection)
            {
                return _conn.Query<DbColumnDef>(string.Format("pragma table_info({0})", tableName)).ToList();
            }
#else
            return new List<DbColumnDef>();
#endif
        }
    }
}