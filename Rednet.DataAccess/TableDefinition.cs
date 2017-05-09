using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Rednet.DataAccess
{
    public class TableDefinition
    {
        private Type m_Type = null;
        private string m_BaseName;

        private IDataFunctions m_DefaultDataFunctions = null;
        private ObjectDefAttribute m_ObjectDefAttribute = null;

        private static int m_CountTables = 0;
        private static readonly Dictionary<string, TableDefinition> TableDefinitions = new Dictionary<string, TableDefinition>();
        private string m_StatementSelect = string.Empty;
        private string m_StatementSelectForReload = string.Empty;

        private static List<SqlStatements> m_SqlList = new List<SqlStatements>();

        private TableDefinition() { }

        public static TableDefinition GetTableDefinition(Type type)
        {
#if !PCL
#if WINDOWS_PHONE_APP
            return GetTableDefinition(type.GetTypeInfo().IsGenericType ? type.GenericTypeArguments[0] : type, "");
#else
            return GetTableDefinition(type.IsGenericType ? type.GenericTypeArguments[0] : type, "");
#endif
#else
            return GetTableDefinition(type, "");
#endif
        }

        private static TableDefinition GetTableDefinition(Type type, string baseName)
        {

            var _nameKey = string.Format("{0}{1}", (baseName.IsNullOrEmpty() ? "" : baseName + "_"), type.Name);

            if (!TableDefinitions.ContainsKey(_nameKey))
            {
                var _nameAlias = string.Format("{0}_{1}", _nameKey, GetNext());
                var _new = new TableDefinition
                {
                    BaseType = type, 
                    BaseName = baseName, 
                    TableName = type.Name, 
                    TableNameAlias = _nameAlias,
                };
                _new.InitFields();

                TableDefinitions.Add(_nameKey, _new);
            }

            return TableDefinitions[_nameKey];

        }

        public IDataFunctions DefaultDataFunction
        {
            get
            {
                if (m_DefaultDataFunctions == null)
                    this.ResolveDefaultDataFunction();

                return m_DefaultDataFunctions;
            }
        }

        private void ResolveDefaultDataFunction()
        {
            if (DatabaseObjectShared.DataFunctions.Count <= 0) return;

            var _att = this.ObjectDefAttribute;
            m_DefaultDataFunctions = DatabaseObjectShared.DataFunctions[_att == null ? DatabaseObjectShared.DefaultDataFunctionName : _att.DefaultDataFunctionName];
        }

        public ObjectDefAttribute ObjectDefAttribute
        {
            get { return m_ObjectDefAttribute; }
        }

        private void ResolveObjectDefAttribute()
        {
#if !PCL
#if WINDOWS_PHONE_APP
            m_ObjectDefAttribute = this.BaseType.GetTypeInfo().GetCustomAttribute<ObjectDefAttribute>() ?? new ObjectDefAttribute();
#else
            m_ObjectDefAttribute = this.BaseType.GetCustomAttribute<ObjectDefAttribute>() ?? new ObjectDefAttribute();
#endif
#endif
        }

        private static int GetNext()
        {
            m_CountTables++;
            return m_CountTables;
        }

        public string TableName { get; private set; }
        public string TableNameAlias { get; private set; }
        public Dictionary<string, TableDefinition> JoinTables { get; private set; }
        public Dictionary<string, FieldDefAttribute> Fields { get; private set; }
        public Dictionary<string, FieldRuleAttribute> Rules { get; private set; }
        public Dictionary<string, JoinFieldAttribute> JoinFields { get; private set; }

        public string BaseName
        {
            get { return m_BaseName; }
            set { m_BaseName = value; }
        }

        public Type BaseType
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public void InitFields()
        {
#if !PCL
            this.ResolveObjectDefAttribute();
            this.ResolveDefaultDataFunction();

            this.JoinTables = new Dictionary<string, TableDefinition>();
            this.Fields = new Dictionary<string, FieldDefAttribute>();
            this.Rules = new Dictionary<string, FieldRuleAttribute>();
            this.JoinFields = new Dictionary<string, JoinFieldAttribute>();
#if WINDOWS_PHONE_APP
            var _srcProp = m_Type.GetRuntimeProperties();
#else
            var _srcProp = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
#endif
            foreach (var p in _srcProp)
            {
                try
                {
                    if (!p.CanWrite) continue;

                    var _p = _srcProp.FirstOrDefault(d => d.Name == p.Name);

                    if (_p == null) continue;

                    var _joinAtt = (JoinFieldAttribute)_p.GetCustomAttributes(typeof(JoinFieldAttribute), true).FirstOrDefault();

                    if (_joinAtt != null)
                    {
#if WINDOWS_PHONE_APP
                        if (_p.PropertyType.GetTypeInfo().IsGenericType)
#else
                        if (_p.PropertyType.IsGenericType)
#endif
                            _joinAtt.FieldType = _p.PropertyType.GenericTypeArguments[0];
                        else
                            _joinAtt.FieldType = _p.PropertyType;

                        _joinAtt.Name = _p.Name;
                        _joinAtt.TableName = _joinAtt.FieldType.Name;
                        _joinAtt.SourceTableName = m_Type.Name;

#if WINDOWS_PHONE_APP
                        var _props = _joinAtt.FieldType.GetRuntimeProperties().Where(pi => pi.CanWrite);
#else
                        var _props = _joinAtt.FieldType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.CanWrite);
#endif

                        var _fieldNamesProps = _props.Where(pi => !pi.GetCustomAttributes<JoinFieldAttribute>().Any());
                        _joinAtt.FieldNames = _fieldNamesProps.Select(pi => pi.Name).ToArray();

                        var _fields = new List<string>();

                        foreach (var _prop in _props)
                        {
                            var _flds = _prop.GetCustomAttributes<FieldDefAttribute>(true).FirstOrDefault(pi => pi.IsPrimaryKey);

                            if (_flds != null)
                            {
                                if (_flds.IgnoreForSave) continue;

                                _fields.Add(_prop.Name);
                            }
                        }
                        _joinAtt.PrimaryKeysFields = _fields.ToArray();

                        this.JoinFields.Add(_p.Name, _joinAtt);
                        this.JoinTables.Add(_joinAtt.Name, TableDefinition.GetTableDefinition(_joinAtt.FieldType, this.TableName));
                    }
                    else
                    {
                        var _dbAtt = (FieldDefAttribute)_p.GetCustomAttributes(typeof(FieldDefAttribute), true).FirstOrDefault();
                        var _ignore = _dbAtt != null && _dbAtt.IgnoreForSave;

                        if (!_ignore)
                        {
                            if (_dbAtt != null)
                            {
                                if (_dbAtt.IsInternal) continue;

                                this.Fields.Add(_p.Name, new FieldDefAttribute
                                {
                                    IsPrimaryKey = _dbAtt.IsPrimaryKey,
                                    AutomaticValue = _dbAtt.AutomaticValue,
                                    Name = _p.Name,
                                    DotNetType = _p.PropertyType,
                                    IsNullAble = _dbAtt.IsNullAble,
                                    Lenght = _dbAtt.Lenght,
                                    Label = _dbAtt.Label,
                                    ShortLabel = _dbAtt.ShortLabel,
                                    DisplayOnForm = _dbAtt.DisplayOnForm,
                                    DisplayOnGrid = _dbAtt.DisplayOnGrid,
                                    IgnoreForSave = _dbAtt.IgnoreForSave,
                                    Precision = _dbAtt.Precision,
                                    EditOnForm = _dbAtt.EditOnForm,
                                    SerializeField = _dbAtt.SerializeField,
                                    NumberFormat = _dbAtt.NumberFormat,
                                    ValidChars = _dbAtt.ValidChars
                                });
                            }
                            else
                            {
                                this.Fields.Add(_p.Name, new FieldDefAttribute {IsPrimaryKey = false, AutomaticValue = AutomaticValue.None, Name = _p.Name, DotNetType = _p.PropertyType, IsNullAble = true, Lenght = 0, EditOnForm = true, SerializeField = false, ValidChars = "", NumberFormat = ""});
                            }
                        }

                        var _ruleAtt = (FieldRuleAttribute)_p.GetCustomAttributes(typeof(FieldRuleAttribute), true).FirstOrDefault();

                        if (_ruleAtt == null)
                            this.Rules.Add(_p.Name, new FieldRuleAttribute {Name = _p.Name, Type = _p.PropertyType, ValidationText = "", IsForValidate = false});
                        else
                        {
                            _ruleAtt.Name = _p.Name;
                            _ruleAtt.Type = _p.PropertyType;
                            _ruleAtt.IsForValidate = true;
                            this.Rules.Add(_p.Name, _ruleAtt);
                        }
                    }
                }
                catch (FormatException fex)
                {
                }
                catch (Exception ex)
                {
                    throw new Exception("Validar erro na propriedade InnerException", ex);
                }
            }
#endif
        }

        public string GetScriptInsert(IDictionary<string, object> data)
        {

            var _sql = new StringBuilder();

            _sql.AppendFormat(" insert into {0} \r\n\t(", this.TableName);

            var _first = true;
            foreach (var _field in this.Fields.Select(f => f.Value))
            {
                _sql.Append(_first ? "" : ", ");
                _sql.AppendFormat(" {0}", _field.Name);
                _first = false;
            }
            _sql.Append(")\r\n values \r\n\t(");

            _first = true;
            foreach (var _field in this.Fields.Select(f => f.Value))
            {
                _sql.Append(_first ? "" : ", ");
                _sql.Append(this.GetValueFromType(_field.DotNetType, data[_field.Name]));
                _first = false;
            }
            _sql.AppendLine(")");

            return _sql.ToString();
        }

        public string GetScriptUpdate(IDictionary<string, object> data)
        {

            var _sql = new StringBuilder();

            _sql.AppendFormat(" update {0} set\r\n", this.TableName);

            var _first = true;
            foreach (var _field in this.Fields.Where(f => !f.Value.IsPrimaryKey).Select(f => f.Value))
            {
                _sql.Append(_first ? "" : ",\r\n");
                _sql.AppendFormat(" {0} = ", _field.Name);
                _sql.Append(this.GetValueFromType(_field.DotNetType, data[_field.Name]));
                _first = false;
            }
            _sql.AppendLine("\r\n where 1 = 1");

            foreach (var _field in this.Fields.Where(f => f.Value.IsPrimaryKey).Select(f => f.Value))
            {
                _sql.AppendFormat(" and {0} = ", _field.Name);
                _sql.Append(this.GetValueFromType(_field.DotNetType, data[_field.Name]));
            }

            return _sql.ToString();
        }

        public string GetScriptDelete(IDictionary<string, object> data)
        {

            var _sql = new StringBuilder();

            _sql.AppendFormat(" delete from {0}\r\n", this.TableName);
            _sql.AppendLine(" where 1 = 1");

            foreach (var _field in this.Fields.Where(f => f.Value.IsPrimaryKey).Select(f => f.Value))
            {
                _sql.AppendFormat(" and {0} = ", _field.Name);
                _sql.Append(this.GetValueFromType(_field.DotNetType, data[_field.Name]));
            }

            return _sql.ToString();
        }

        public string GetSqlSelect()
        {
#if !PCL
            var _type = SqlStatementsTypes.Select;
            var _sql = GetSqlStatement(_type);

            if (!_sql.Equals("")) return _sql;
            var _ret = new StringBuilder();

            _ret.AppendFormat(" select * from {0}\n where 1 = 1", this.TableName);
            foreach (var _objectField in this.Fields.Select(f=>f.Value).Where(f => f.IsPrimaryKey))
            {
                _ret.AppendFormat(" and {0} = {1}\n", _objectField.Name, _objectField.GetParameterName(this.DefaultDataFunction));
            }

            _sql = _ret.ToString();
            SetSqlStatement(_type, _sql);

            return _sql;
#else
            return "";
#endif
        }

        public string GetSqlSelectForCheck()
        {
#if !PCL
            var _type = SqlStatementsTypes.SelectCheck;
            var _sql = GetSqlStatement(_type);

            if (!_sql.Equals("")) return _sql;

            var _ret = new StringBuilder();

            _ret.AppendFormat(" select count(0) as tt from {0}\n where 1 = 1", this.TableName);
            foreach (var _objectField in this.Fields.Select(f=>f.Value).Where(f => f.IsPrimaryKey))
            {
                _ret.AppendFormat(" and {0} = {1}\n", _objectField.Name, _objectField.GetParameterName(this.DefaultDataFunction));
            }

            _sql = _ret.ToString();
            SetSqlStatement(_type, _sql);

            return _sql;
#else
            return "";
#endif
        }

        public string GetSqlInsert(bool ignoreAutoIncrementField = true)
        {
#if !PCL
            var _type = SqlStatementsTypes.Insert;
            var _sql = GetSqlStatement(_type);

            if (!_sql.Equals("")) return _sql;

            var _ret = new StringBuilder();

            var _fields = this.Fields.Select(f => f.Value).ToList();

            if (ignoreAutoIncrementField)
                _fields = _fields.Where(o => o.AutomaticValue == AutomaticValue.None).ToList();

            _ret.AppendFormat(" insert into {0}\n (", this.TableName);
            string _sep = "";
            foreach (var _objectField in _fields)
            {
                _ret.AppendFormat("{0}{1}", _sep, _objectField.Name);
                _sep = ", ";
            }
            _ret.Append(")\n values\n (");
            _sep = "";
            foreach (var _objectField in _fields)
            {
                _ret.AppendFormat("{0}{1}", _sep, _objectField.GetParameterName(this.DefaultDataFunction));
                _sep = ", ";
            }
            _ret.Append(")\n");

            _sql = _ret.ToString();
            SetSqlStatement(_type, _sql);

            return _sql;
#else
            return "";
#endif
        }

        public string GetSqlUpdate()
        {
#if !PCL
            var _type = SqlStatementsTypes.Update;
            var _sql = GetSqlStatement(_type);

            if (!_sql.Equals("")) return _sql;

            var _ret = new StringBuilder();

            _ret.AppendFormat(" update {0} set\n", this.TableName);
            string _sep = "";
            foreach (var _objectField in this.Fields.Select(f=>f.Value).Where(f => !f.IsPrimaryKey))
            {
                _ret.AppendFormat("{0} {1} = {2}", _sep, _objectField.Name, _objectField.GetParameterName(this.DefaultDataFunction));
                _sep = ",\n";
            }

            _ret.AppendLine("\n where 1 = 1");
            foreach (var _objectField in this.Fields.Select(f => f.Value).Where(f => f.IsPrimaryKey))
            {
                _ret.AppendFormat(" and {0} = {1}\n", _objectField.Name, _objectField.GetParameterName(this.DefaultDataFunction));
            }

            _sql = _ret.ToString();
            SetSqlStatement(_type, _sql);

            return _sql;
#else
            return "";
#endif
        }

        public string GetSqlDelete()
        {
#if !PCL
            var _type = SqlStatementsTypes.Delete;
            var _sql = GetSqlStatement(_type);

            if (!_sql.Equals("")) return _sql;

            var _ret = new StringBuilder();

            _ret.AppendFormat(" delete from {0}\n where 1 = 1\n", this.TableName);
            foreach (var _objectField in this.Fields.Select(f=>f.Value).Where(f => f.IsPrimaryKey))
            {
                _ret.AppendFormat(" and {0} = {1}\n", _objectField.Name, _objectField.GetParameterName(this.DefaultDataFunction));
            }

            _sql = _ret.ToString();
            SetSqlStatement(_type, _sql);

            return _sql;
#else
            return "";
#endif
        }

        private void SetSqlStatement(SqlStatementsTypes type, string sql)
        {
            var _sql = m_SqlList.FirstOrDefault(s => s.TableName == this.TableName && s.Type == type) ?? new SqlStatements() { Type = type, TableName = this.TableName };
            _sql.SqlStatement = sql;
            m_SqlList.Add(_sql);
        }

        private string GetSqlStatement(SqlStatementsTypes type)
        {
            var _sql = m_SqlList.FirstOrDefault(s => s.TableName == this.TableName && s.Type == type);

            return _sql == null ? "" : _sql.SqlStatement;
        }

        private string GetValueFromType(Type type, object value)
        {
            if (value == null)
                return "null";

            var _name = "";
#if !PCL
#if WINDOWS_PHONE_APP
            _name = type.GetTypeInfo().IsGenericType ? type.GenericTypeArguments[0].Name : type.Name;
#else
            _name = type.IsGenericType ? type.GenericTypeArguments[0].Name : type.Name;
#endif
#else
            _name = type.Name;
#endif
            switch (_name.ToLower())
            {
                case "string":
                case "char":
                    return string.Format("'{0}'", value);

                case "datetime":
                    var _date = Convert.ToDateTime(value);
                    return string.Format(this.DefaultDataFunction.GetDateTimeFormat(), _date.ToString("yyyy-MM-dd HH:mm:ss"));

                case "boolean":
                    return string.Format("{0}", Convert.ToInt32(value));

                case "int16":
                case "int32":
                case "int64":
                case "float":
                case "decimal":
                case "double":
                    return string.Format("{0}", value.ToString().Replace(",", "."));

                default:
                    return string.Format("{0}", value);
            }
        }

        public string GetStatementSelect(bool getForReloadMe = false)
        {

            if (getForReloadMe)
            {
                if (!m_StatementSelectForReload.IsNullOrEmpty())
                    return m_StatementSelectForReload;
            }
            else
            {
                if (!m_StatementSelect.IsNullOrEmpty())
                    return m_StatementSelect;
            }

            var _sql = new StringBuilder();

            _sql.AppendLine(" select");

            var _first = true;
            foreach (var _field in this.Fields)
            {
                _sql.Append(_first ? "" : ",\r\n");
                _sql.AppendFormat(" {0}.{1} as {1}", this.TableNameAlias, _field.Value.Name);
                _first = false;
            }

            foreach (var _joinTable in this.JoinTables)
            {
                _sql.Append(this.GetSqlFields(_joinTable.Value, _joinTable.Key));
            }

            _sql.AppendLine(string.Format("\r\n from {0} {1}", this.TableName, this.TableNameAlias));

            _sql.AppendLine(this.GetJoinStatement(this, this.JoinTables));

            if (getForReloadMe)
            {
                _sql.AppendLine(" where 1 = 1");
                _sql.Append(this.GetWhereClauseWithPrimaryKey());
            }

            if (getForReloadMe)
                m_StatementSelectForReload = _sql.ToString();
            else
                m_StatementSelect = _sql.ToString();

            return _sql.ToString();
        }

        private string GetSqlFields(TableDefinition joinTable, string key)
        {
            var _ret = new StringBuilder();
            foreach (var _field in joinTable.Fields)
            {
                _ret.Append(",\r\n");
                _ret.AppendFormat(" {0}.{1} as {2}_{1}", joinTable.TableNameAlias, _field.Value.Name, key);
            }

            foreach (var _joinTable in joinTable.JoinTables)
            {
                _ret.Append(this.GetSqlFields(_joinTable.Value, key + "_" + _joinTable.Key));
            }

            return _ret.ToString();

        }

        public string ReplaceTableAlias(string whereDef, TableDefinition tableDef)
        {
            var _find = string.Format("({0}.", tableDef.TableName);
            var _replace = string.Format("({0}.", tableDef.TableNameAlias);
            var _ret = whereDef.Replace(_find, _replace);

            foreach (var _table in tableDef.JoinTables)
            {
                _ret = this.ReplaceTableAlias(_ret, _table.Value);
            }
            return _ret;
        }

        private string GetJoinStatement(TableDefinition baseTable, Dictionary<string, TableDefinition> joinTables)
        {
            var _ret = new StringBuilder();
            foreach (var _joinField in baseTable.JoinFields.Select(b=>b.Value))
            {
                var _joinRelation = "";
                for (var _i = 0; _i < _joinField.SourceColumnNames.Length; _i++)
                {
                    _joinRelation += (_joinRelation == "" ? "" : " and ") + string.Format("{0}.{1} = {2}.{3}", baseTable.TableNameAlias, _joinField.SourceColumnNames[_i], joinTables[_joinField.Name].TableNameAlias, _joinField.TargetColumnNames[_i]);
                }

                var _join = "";
                switch (_joinField.JoinType)
                {
                    case JoinType.InnerJoin:
                        _join += " inner join ";
                        break;
                    case JoinType.LeftJoin:
                        _join += " left outer join ";
                        break;
                    case JoinType.RightJoin:
                        _join += " right outer join ";
                        break;
                }

                _join += string.Format("{0} {1} on {2}", _joinField.TableName, joinTables[_joinField.Name].TableNameAlias, _joinRelation);
                
                _ret.AppendLine(_join);

            }

            foreach (var _joinTable in baseTable.JoinTables)
            {
                _ret.AppendLine(this.GetJoinStatement(_joinTable.Value, _joinTable.Value.JoinTables));
            }

            return _ret.ToString();

        }

        private string GetWhereClauseWithPrimaryKey()
        {
            var _ret = new StringBuilder();
            foreach (var _field in this.Fields.Where(f => f.Value.IsPrimaryKey))
            {
                _ret.AppendLine(string.Format(" and {0}.{1} = {2}{1}", this.TableNameAlias, _field.Value.Name, this.DefaultDataFunction.PrefixParameter));
            }
            return _ret.ToString();
        }

        public string GetScriptCreateTable()
        {
            var _ddl = new StringBuilder();

            _ddl.AppendFormat(" create table if not exists \"{0}\"\r\n", this.TableName);
            _ddl.AppendLine();
            _ddl.AppendLine(" (");

            var _fields = this.Fields.Select(f => f.Value);
            var _pks = _fields.Where(o => o.IsPrimaryKey);

            var _first = true;
            foreach (var _field in _fields)
            {
                _ddl.Append(_first ? "" : ", ");
                _ddl.AppendFormat("     {0} {1}", _field.Name, GetDBType(_field));

                if (_first)
                {
                    if (_pks.Count() == 1)
                    {
                        _ddl.Append(" primary key");
                        if (_field.AutomaticValue == AutomaticValue.AutoIncrement)
                            _ddl.Append(" autoincrement");
                    }
                }

                if (!_field.IsNullAble)
                    _ddl.Append(" not null");

                _first = false;
            }

            if (_pks.Count() > 1)
            {
                _ddl.AppendLine();
                _ddl.Append(",     primary key (");
                _first = true;
                foreach (var _field in _pks)
                {
                    _ddl.Append(_first ? "" : ", ");
                    _ddl.Append(_field.Name);
                    _first = false;
                }
                _ddl.AppendLine(")");
            }

            _ddl.AppendLine(")");

            return _ddl.ToString();
        }

        public string GetScriptDropTable()
        {
            return string.Format(" drop table {0} {1}", (this.DefaultDataFunction.DatabaseType == DatabaseType.SQLite ? "if exists" : ""), this.TableName);
        }
        public string GetScriptTruncateTable()
        {
            return string.Format(" truncate table {0}", this.TableName);
        }

        private static string GetDBType(FieldDefAttribute fieldDef)
        {
#if !PCL
#if WINDOWS_PHONE_APP
            var _type = (fieldDef.DotNetType.GetTypeInfo().IsEnum ? "enum" : fieldDef.DotNetType.Name.ToLower());
#else
            var _type = (fieldDef.DotNetType.IsEnum ? "enum" : fieldDef.DotNetType.Name.ToLower());
#endif
            if (_type.Contains("nullable"))
            {
                var _t = Nullable.GetUnderlyingType(fieldDef.DotNetType);
#if WINDOWS_PHONE_APP
                _type = _t.GetTypeInfo().IsEnum ? "enum" : _t.Name.ToLower();
#else
                _type = _t.IsEnum ? "enum" : _t.Name.ToLower();
#endif
                fieldDef.IsNullAble = true;
            }

            switch (_type)
            {
                case "char":
                case "string": // STRING, CHAR, ALFANUMERIC
                    if (fieldDef.Lenght == 1)
                        return "char(1)";
                    else
                    {
                        var _len = fieldDef.Lenght;
                        return string.Format("varchar({0})", _len == 0 ? 255 : _len);
                    }

                case "bool":
                case "boolean":
                case "enum":
                case "byte":
                case "int":
                case "int16":
                case "int32":
                case "int64":
                case "long":
                    return "integer";

                case "datetime":
                    return "date";

                case "double":
                case "decimal":
                case "float":
                    return "float";

                case "byte[]":
                    return "blob";

                default:
                    throw new Exception("Tipo não tratado para esse valor");
            }
#else
                return "";
#endif
        }
    }
}