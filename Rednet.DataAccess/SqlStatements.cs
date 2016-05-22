namespace Rednet.DataAccess
{
    public enum SqlStatementsTypes
    {
        None,
        Select,
        SelectCheck,
        SelectReload,
        Insert,
        Update,
        Delete,
        DeleteAll,
        UnknownStatement
    }

    public class SqlStatements
    {
        public string TableName { get; set; }
        public SqlStatementsTypes Type { get; set; }
        public string SqlStatement { get; set; }
    }
}