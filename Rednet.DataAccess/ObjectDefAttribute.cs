using System;

namespace Rednet.DataAccess
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ObjectDefAttribute : Attribute
    {
        private string m_DatabaseName = DatabaseObjectShared.DatabaseName;
        public string TableName { get; set; }

        public string DatabaseName  
        {
            get { return m_DatabaseName; }
            set { m_DatabaseName = value; }
        }

        public bool PrefixTableNameWithDatabaseName { get; set; }
    }
}