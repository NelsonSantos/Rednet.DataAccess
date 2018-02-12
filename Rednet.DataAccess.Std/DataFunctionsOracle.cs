using System.Collections.Generic;
#if !PCL
using System.Data;
using System.Data.OracleClient;
#endif
using System.Linq;
using Oracle.DataAccess.Client;

namespace Rednet.DataAccess
{
    public class DataFunctionsOracle : DataFunctions<DataFunctionsOracle>
    {
        public DataFunctionsOracle()
            : base(DatabaseType.Oracle)
        {
        }

        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

#if !PCL
        public override IDbConnection Connection
        {
            get
            {
                var _connString = this.GetConnectionString();
                return new OracleConnection(_connString);
            }
        }
#endif

        public override string GetConnectionString()
        {
            return string.Format("Data Source={0};User Id={1};Password={2};Pooling={3};", this.DataSource, this.UserId, this.Password, this.Pooling);
        }

        public override string GetSqlLastIdentity()
        {
            return "; select last_insert_rowid() as ultid";
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
            get { return ":"; }
        }
    }
}