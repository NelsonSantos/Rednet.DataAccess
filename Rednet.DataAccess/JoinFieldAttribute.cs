using System;
using System.Collections.Generic;

namespace Rednet.DataAccess
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JoinFieldAttribute : BaseFieldDef
    {
        private readonly Dictionary<string, object> m_FilterParameters = new Dictionary<string, object>();
        public JoinFieldAttribute()
        {
            this.IgnoreForSave = true;
        }

        public string[] SourceColumnNames { get; set; } = new string[0];
        public string[] TargetColumnNames { get; set; } = new string[0];
        public JoinType JoinType { get; set; }
        public JoinRelation JoinRelation { get; set; }
        internal Type FieldType { get; set; }
        internal string TableName { get; set; }
        internal string SourceTableName { get; set; }
        internal string SourceTableNameAlias { get; set; }
        internal string[] PrimaryKeysFields { get; set; } = new string[0];
        internal string[] FieldNames { get; set; } = new string[0];
        public string[] FilterColumnNames { get; set; } = new string[0];
        public object[] FilterColumnValues { get; set; } = new object[0];

        internal void AddFilterParameter(string parameterName, object parameterValue)
        {
            m_FilterParameters.Add(parameterName, parameterValue);
        }

        internal Dictionary<string, object> GetFilterParameters()
        {
            return m_FilterParameters;
        }

        //public string SplitOnField { get; internal set; }
        //public string PrefixTable { get; internal set; }
        //public string TableNameAlias { get; internal set; }
    }
}