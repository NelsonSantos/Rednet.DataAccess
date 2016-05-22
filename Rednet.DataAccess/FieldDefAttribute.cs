using System;
using System.Linq;
using System.Linq.Expressions;

namespace Rednet.DataAccess
{

    public enum AutomaticValue
    {
        None,
        AutoIncrement,
        BackEndCalculated
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FieldDefAttribute : Attribute //, IFieldDefinition
    {
        private bool m_IsNullAble = true;
        private AutomaticValue m_AutomaticValue = AutomaticValue.None;
        private bool m_DisplayOnGrid = true;
        private bool m_DisplayOnForm = true;
        private bool m_IsInternal = false;
        private bool m_EditOnForm = true;

        public AutomaticValue AutomaticValue
        {
            get { return m_AutomaticValue; }
            set { m_AutomaticValue = value; }
        }

        public bool IsNullAble
        {
            get
            {
                if (IsPrimaryKey)
                    return false;

                if (m_AutomaticValue == AutomaticValue.AutoIncrement)
                    return false;

                return m_IsNullAble;
            }
            set { m_IsNullAble = value; }
        }

        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IgnoreForSave { get; set; }

        public bool DisplayOnForm
        {
            get { return m_DisplayOnForm; }
            set { m_DisplayOnForm = value; }
        }

        public bool DisplayOnGrid
        {
            get { return m_DisplayOnGrid; }
            set { m_DisplayOnGrid = value; }
        }

        public bool IsInternal
        {
            get { return m_IsInternal; }
            set { m_IsInternal = value; }
        }

        public bool EditOnForm
        {
            get { return m_EditOnForm; }
            set { m_EditOnForm = value; }
        }

        public string Label { get; set; }
        public string ShortLabel { get; set; }
        public Type DotNetType { get; set; }
        public int Lenght { get; set; }
        public int Precision { get; set; }
        public string GetParameterName(IDataFunctions datafunction)
        {
            return string.Format("{0}{1}", datafunction.PrefixParameter, this.Name);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class JoinFieldAttribute : FieldDefAttribute
    {
        public JoinFieldAttribute()
        {
            this.IgnoreForSave = true;
        }

        public string[] SourceColumnNames { get; set; }
        public string[] TargetColumnNames { get; set; }
        public JoinType JoinType { get; set; }
        public JoinRelation JoinRelation { get; set; }
        public Type FieldType { get; internal set; }
        //public string SplitOnField { get; internal set; }
        public string PrefixTable { get; internal set; }
        public string TableName { get; internal set; }
        public string TableNameAlias { get; internal set; }
        public string SourceTableName { get; internal set; }
        public string SourceTableNameAlias { get; internal set; }
        public string[] PrimaryKeysFields { get; internal set; }
        public string[] FieldNames { get; internal set; }
    }

    public enum JoinType
    {
        InnerJoin,
        LeftJoin,
        RightJoin
    }

    public enum JoinRelation
    {
        OneToOne,
        OneToMany
    }
}