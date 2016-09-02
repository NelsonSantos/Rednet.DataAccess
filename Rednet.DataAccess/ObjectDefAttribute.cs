using System;

namespace Rednet.DataAccess
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ObjectDefAttribute : Attribute
    {
        private string m_DefaultDataFunctionName = DatabaseObjectShared.DefaultDataFunctionName;
        public string TableName { get; set; }

        public string DefaultDataFunctionName
        {
            get { return m_DefaultDataFunctionName; }
            set { m_DefaultDataFunctionName = value; }
        }

        public string DatabaseName { get; set; }

        public bool PrefixTableNameWithDatabaseName { get; set; }
    }
}