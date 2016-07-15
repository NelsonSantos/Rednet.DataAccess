namespace System.Data
{
    public class DataException : Exception
    {
        public DataException()
            : base("A Data exception has occurred")
        {
        }

        public DataException(string s) : base(s)
        {
        }

        public DataException(string s, Exception innerException)
            : base(s, innerException)
        {
        }
    }
}