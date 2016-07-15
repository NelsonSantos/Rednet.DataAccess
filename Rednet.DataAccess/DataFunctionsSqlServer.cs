using System.Collections.Generic;
#if !PCL
using System.Data;
using System.Data.SqlClient;
#endif

namespace Rednet.DataAccess
{

    /// <summary>
    /// 
    /// </summary>
    public class DataFunctionsSqlServer : DataFunctions<DataFunctionsSqlServer>
    {
        public DataFunctionsSqlServer()
            : base(DatabaseType.SQLServer)
        {
        }

        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool IsTrustedConnection { get; set; }

#if !PCL
        public override IDbConnection Connection
        {
            get
            {
                var _connString = this.GetConnectionString();
                return new SqlConnection(_connString);
            }
        }
#endif

        public override string GetConnectionString()
        {
            if (IsTrustedConnection)
                return string.Format("Server={0};Database={1};Trusted_Connection=True;Pooling={2};", this.Server, this.Database, this.Pooling);

            return string.Format("Server={0};Database={1};User Id={2};Password={3};Pooling={4};", this.Server, this.Database, this.UserId, this.Password, this.Pooling);
        }

        public override string GetSqlLastIdentity()
        {
            return "; select @@IDENTITY as ultid";
        }

        public override string GetDateTimeFormat()
        {
            throw new System.NotImplementedException();
        }

        public override List<DbColumnDef> GetColumnsDef(string tableName)
        {
            return new List<DbColumnDef>();
        }

        public override string PrefixParameter
        {
            get { return "@"; }
        }
    }
}