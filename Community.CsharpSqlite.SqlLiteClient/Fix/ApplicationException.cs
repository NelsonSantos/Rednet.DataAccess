using System;

namespace Community.CsharpSqlite.SQLiteClient 
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string msg) : base(msg) { }

        public ApplicationException(string msg, Exception ex) : base(msg, ex) { }
    }
}
