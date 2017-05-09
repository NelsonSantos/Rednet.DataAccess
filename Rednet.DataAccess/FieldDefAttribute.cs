using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rednet.DataAccess
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldDefAttribute : BaseFieldDef
    {
        private bool m_IsNullAble = true;
        private Type m_ObjectType = null;

        public AutomaticValue AutomaticValue { get; set; } = AutomaticValue.None;

        public bool IsNullAble
        {
            get
            {
                if (IsPrimaryKey)
                    return false;

                if (this.AutomaticValue == AutomaticValue.AutoIncrement)
                    return false;

                return m_IsNullAble;
            }
            set { m_IsNullAble = value; }
        }

        public bool IsPrimaryKey { get; set; }
        internal Type DotNetType { get; set; }
        public int Lenght { get; set; }
        public int Precision { get; set; }
        public string NumberFormat { get; set; } = "";
        public string ValidChars { get; set; } = "";
        public string GetParameterName(IDataFunctions datafunction)
        {
            return $"{datafunction.PrefixParameter}{this.Name}";
        }
        public object GetValue(object obj)
        {
            if (m_ObjectType == null)
                m_ObjectType = obj.GetType();

            return m_ObjectType.GetRuntimeProperty(this.Name).GetValue(obj);
        }
    }
}