using System;

namespace Rednet.DataAccess
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JoinFieldAttribute : BaseFieldDef
    {
        public JoinFieldAttribute()
        {
            this.IgnoreForSave = true;
        }

        public string[] SourceColumnNames { get; set; }
        public string[] TargetColumnNames { get; set; }
        public JoinType JoinType { get; set; }
        public JoinRelation JoinRelation { get; set; }
        internal Type FieldType { get; /*internal*/ set; }
        //public string SplitOnField { get; internal set; }
        //public string PrefixTable { get; internal set; }
        internal string TableName { get; /*internal*/ set; }
        //public string TableNameAlias { get; internal set; }
        internal string SourceTableName { get; /*internal*/ set; }
        internal string SourceTableNameAlias { get; /*internal*/ set; }
        internal string[] PrimaryKeysFields { get; /*internal*/ set; }
        internal string[] FieldNames { get; /*internal*/ set; }
    }
}