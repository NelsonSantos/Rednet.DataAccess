using System.Collections.Generic;
#if !PCL
using System.Data;
using System.Data.OracleClient;
using MySql.Data.MySqlClient;
#endif
using System.Linq;

namespace Rednet.DataAccess
{
    public class DataFunctionsMySql : DataFunctions<DataFunctionsMySql>
    {
        private int m_Port = 3306;

        public DataFunctionsMySql()
            : base(DatabaseType.MySQL)
        {
        }

        public string Server { get; set; }

        public int Port
        {
            get { return m_Port; }
            set { m_Port = value; }
        }

        public string DataBase { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool IncludeSecurityAsserts { get; set; }


#if !PCL
        public override IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(this.GetConnectionString());
            }
        }
#endif

        public override string GetConnectionString()
        {
            return string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};Pooling={5};IncludeSecurityAsserts={6};AllowZeroDateTime=True;ConvertZeroDateTime=True;", this.Server, this.Port, this.DataBase, this.UserId, this.Password, this.Pooling, this.IncludeSecurityAsserts);
        }

        public override string GetSqlLastIdentity()
        {
            return "; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS ID";
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