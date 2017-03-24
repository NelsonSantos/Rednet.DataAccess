using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
//using System.Linq.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#if !PCL
using Rednet.DataAccess.FastMember;
using Rednet.DataAccess.Dapper;
using System.Data;
using System.Data.Common;
#endif

namespace Rednet.DataAccess
{

#if !PCL && !WINDOWS_PHONE_APP
    [Serializable]
#endif
    public abstract class DatabaseObject<T> : IDatabaseObject, INotifyPropertyChanged
    {

#if !PCL && !WINDOWS_PHONE_APP
        [field: NonSerialized]
#endif
            public event EventHandler<NotifyRecordChangesEventArgs> NotifyRecordChangesAfter;

#if !PCL && !WINDOWS_PHONE_APP
        [field: NonSerialized]
#endif
            public event EventHandler<NotifyRecordChangesEventArgs> NotifyRecordChangesBefore;

        private static List<SqlStatements> m_SqlList = new List<SqlStatements>();
        private static object m_LockObject = new object();

#if !PCL && !WINDOWS_PHONE_APP
        [field: NonSerialized] [JsonIgnore]
#endif
            private Func<bool> m_ValidateDataFunction = null;

        protected virtual void OnBeforeSaveData(NotifyRecordChangesEventArgs e)
        {
            this.NotifyRecordChangesBefore?.Invoke(this, e);
        }

        protected virtual void OnAfterSaveData(NotifyRecordChangesEventArgs e)
        {
            this.NotifyRecordChangesAfter?.Invoke(this, e);
        }

        protected virtual void OnBeforeDeleteData(NotifyRecordChangesEventArgs e)
        {
            this.NotifyRecordChangesBefore?.Invoke(this, e);
        }

        protected virtual void OnAfterDeleteData(NotifyRecordChangesEventArgs e)
        {
            this.NotifyRecordChangesAfter?.Invoke(this, e);
        }

        protected virtual bool OnValidateData()
        {
            var _ret = this.GetValidateDataFunction().Invoke();

            return _ret;
        }

        private Func<bool> GetValidateDataFunction()
        {
#if !PCL
            return m_ValidateDataFunction ?? (m_ValidateDataFunction = (() =>
            {
                var _table = TableDefinition.GetTableDefinition(typeof (T));
                var _rules = _table.Rules.Select(r => r.Value);
                var _validatedFields = new List<ValidatedField>();

                foreach (var _rule in _rules)
                {
                    if (!_rule.IsForValidate) continue;
#if WINDOWS_PHONE_APP
                    var _prop = _table.BaseType.GetRuntimeProperties().FirstOrDefault(f => f.Name == _rule.Name);
#else
                    var _prop = _table.BaseType.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(f => f.Name == _rule.Name);
#endif
                    if (!_rule.Validate(_prop.GetValue(this)))
                        _validatedFields.Add(new ValidatedField() {FieldMessage = _rule.ValidationText, FieldName = _rule.Name});
                }

                if (_validatedFields.Count > 0)
                {
                    this.RaiseErrorOnValidateData(new ErrorOnValidateDataEventArgs(_validatedFields.ToArray()));

                    return false;
                }

                return true;
            }));
#else
            return new Func<bool>(() => false);
#endif
        }

        protected void RaiseErrorOnValidateData(ErrorOnValidateDataEventArgs args)
        {
            this.ErrorOnValidateData?.Invoke(this, args);
        }

        public void SetValidateDataFunction(Func<bool> value)
        {
            m_ValidateDataFunction = value;
        }

#if !PCL && !WINDOWS_PHONE_APP
        [field: NonSerialized]
#endif
        public event ErrorOnSaveOrDeleteEventHandler ErrorOnSaveOrDelete;

#if !PCL && !WINDOWS_PHONE_APP
        [field: NonSerialized]
#endif
        public event ErrorOnValidateDataEventHandler ErrorOnValidateData;

#if !PCL && !WINDOWS_PHONE_APP
        [field: NonSerialized]
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var _handler = PropertyChanged;
            if (_handler != null) _handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public DatabaseObject()
        {
        }

        public static T CreateInstance()
        {
            return Activator.CreateInstance<T>();
        }

        string IDatabaseObject.Name
        {
            get { return DatabaseObject<T>.ObjectName; }
        }

        public static IEnumerable<FieldDefAttribute> GetFields()
        {
            var _table = TableDefinition.GetTableDefinition(typeof (T));
            return _table.Fields.Select(f => f.Value);
        }

#if !PCL && !WINDOWS_PHONE_APP
        [System.ComponentModel.Browsable(false)]
#endif
        [JsonIgnore]
        [FieldDef(DisplayOnForm = false, DisplayOnGrid = false, IgnoreForSave = true, IsInternal = true)]
        public IEnumerable<FieldDefAttribute> Fields
        {
            get { return GetFields(); }
        }

        [FieldDef(DisplayOnForm = false, DisplayOnGrid = false, IgnoreForSave = true)]
        public static string ObjectName
        {
            get
            {
#if !PCL
                var _table = TableDefinition.GetTableDefinition(typeof (T));
                var _def = _table.ObjectDefAttribute;
                var _type = typeof (T);
                return _def.PrefixTableNameWithDatabaseName ? string.Format("{0}.{1}", _def.DatabaseName, _type.Name) : _type.Name;
#else
                return "";
#endif
            }
        }

        private static JsonSerializerSettings m_JsonSerializerSettings = null;

        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return m_JsonSerializerSettings ?? (m_JsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new SerializableContractResolver(),

                Converters =
                {
                    new NumberConverter(),
                    new IsoDateTimeConverter() {DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff"},
                    //new ByteArrayConverter()
                }
            });
        }

        public async Task<string> ToJsonAsync(bool compressString = false)
        {
            return await Task.Run(() => this.ToJson(compressString));
        }

        public string ToJson(bool compressString = false)
        {
            var _ret = JsonConvert.SerializeObject(this, GetJsonSerializerSettings());

            if (compressString)
                _ret = _ret.CompressString();

            return _ret;

        }

        private static Dictionary<string, object> ToDictionary(object value)
        {
            var _type = value.GetType();
            var _ret = new Dictionary<string, object>();

            if (_type.Name.ToLower().Contains("anonymoustype"))
            {
                var _data = DatabaseObject<object>.ToJson(value);
                _ret = JsonConvert.DeserializeObject<Dictionary<string, object>>(_data, GetJsonSerializerSettings());
            }
            else
            {
                var _fields = TableDefinition.GetTableDefinition(_type).Fields.Select(f => f.Value);

                foreach (var _field in _fields)
                {
                    _ret.Add(_field.Name, _field.GetValue(value));
                }
            }

            return _ret;

        }

        public async Task<Dictionary<string, object>> ToDictionaryAsync()
        {
            return await Task.Run(() => this.ToDictionary());
        }

        public Dictionary<string, object> ToDictionary()
        {
            return ToDictionary(this);
        }

        public static async Task<string> ToJsonAsync(T data, bool compressString = false)
        {
            return await Task.Run(() => ToJson(data, compressString));
        }

        public static string ToJson(T data, bool compressString = false)
        {
            var _ret = JsonConvert.SerializeObject(data, GetJsonSerializerSettings());

            if (compressString)
                _ret = _ret.CompressString();

            return _ret;
        }

        public static async Task<string> ToJsonAsync(IEnumerable<T> data, bool compressString = false)
        {
            return await Task.Run(() => ToJson(data, compressString));
        }

        public static string ToJson(IEnumerable<T> data, bool compressString = false)
        {
            var _ret = JsonConvert.SerializeObject(data, GetJsonSerializerSettings());

            if (compressString)
                _ret = _ret.CompressString();

            return _ret;
        }

#if !PCL
        public static async Task<object> FromJsonTypeAsync(Type type, string jsonData, bool decompressString = false)
        {
            return await Task.Run(() => FromJsonType(type, jsonData, decompressString));
        }

        public static object FromJsonType(Type type, string jsonData, bool decompressString = false)
        {
            var _object = typeof (DatabaseObject<>).MakeGenericType(type);
#if WINDOWS_PHONE_APP
            var _method = _object.GetRuntimeMethod("FromJson", new[] {typeof (string), typeof (bool)});
#else
            var _method = _object.GetMethod("FromJson");
#endif
            return _method.Invoke(null, new object[] {jsonData, decompressString});
        }
#endif

        public static async Task<T> FromJsonAsync(string jsonData, bool decompressString = false)
        {
            return await Task.Run(() => FromJson(jsonData, decompressString));
        }

        public static T FromJson(string jsonData, bool decompressString = false)
        {
            var _data = jsonData;

            if (decompressString)
                _data = _data.DecompressString();

            return JsonConvert.DeserializeObject<T>(_data, GetJsonSerializerSettings());
        }

        public static async Task<IEnumerable<T>> FromJsonListAsync(string jsonData, bool decompressString = false)
        {
            return await Task.Run(() => FromJsonList(jsonData, decompressString));
        }

        public static IEnumerable<T> FromJsonList(string jsonData, bool decompressString = false)
        {
            var _data = jsonData;

            if (decompressString)
                _data = _data.DecompressString();

            return JsonConvert.DeserializeObject<IEnumerable<T>>(_data, GetJsonSerializerSettings());
        }

        public async Task<T> CloneAsync()
        {
            return await Task.Run(() => this.Clone());
        }

        public T Clone()
        {
            return JsonConvert.DeserializeObject<T>(this.ToJson(), GetJsonSerializerSettings());
        }

        public async Task<TTarget> CloneToAsync<TTarget>()
        {
            return await Task.Run(() => this.CloneTo<TTarget>());
        }

        public TTarget CloneTo<TTarget>()
        {
            var _json = this.ToJson();
            var _ret = JsonConvert.DeserializeObject<TTarget>(_json, GetJsonSerializerSettings());
            return _ret;
        }

        public string GetScriptInsert()
        {
            var _data = this.ToDictionary();
            return TableDefinition.GetTableDefinition(typeof (T)).GetScriptInsert(_data);
        }

        public string GetScriptUpdate()
        {
            var _data = this.ToDictionary();
            return TableDefinition.GetTableDefinition(typeof (T)).GetScriptUpdate(_data);
        }

        public string GetScriptDelete()
        {
            var _data = this.ToDictionary();
            return TableDefinition.GetTableDefinition(typeof (T)).GetScriptDelete(_data);
        }

        public virtual void SetIdFields()
        {
        }

        string IDatabaseObject.GetCreateTableScript()
        {
            return TableDefinition.GetTableDefinition(typeof (T)).GetScriptCreateTable();
        }

        string IDatabaseObject.GetDropTableScript()
        {
            return TableDefinition.GetTableDefinition(typeof (T)).GetScriptDropTable();
        }

        private void SetSelfColumnsIds(object value)
        {
#if !PCL
            var _table = TableDefinition.GetTableDefinition(typeof (T));
            var _fields = _table.Fields.Select(f => f.Value).Where(f => f.AutomaticValue != AutomaticValue.None);

            foreach (var _field in _fields)
            {
#if WINDOWS_PHONE_APP
                var _prop = value.GetType().GetRuntimeProperties().FirstOrDefault(f => f.Name == _field.Name);
#else
                var _prop = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(f => f.Name == _field.Name);
#endif
                if (_prop != null)
                    this.SetObjectFieldValue(_field.Name, _prop.GetValue(value));
            }
#endif
        }

        private static void ThrowException(CrudReturn status)
        {
            if (status.ReturnStatus == CrudStatus.Fail)
                throw new Exception(status.ReturnMessage);
        }

        public async Task<bool> SaveChangesAsync(bool ignoreAutoIncrementAttribute = true, FireEvent fireEvent = FireEvent.OnBeforeAndAfter, bool doNotUpdateWhenExists = false, bool validateData = false)
        {
            return await Task.Run(() => this.SaveChanges(ignoreAutoIncrementAttribute, fireEvent, doNotUpdateWhenExists, validateData));
        }

        /// <summary>
        /// Save current changes on object
        /// </summary>
        /// <param name="ignoreAutoIncrementAttribute">if true, doesn't include autoincrement or backend calculated fields on insert statement. Defaults to true</param>
        /// <param name="fireEvent">if true, fire the AfterSaveData event. Defaults to true</param>
        /// <param name="doNotUpdateWhenExists">if true, doesn't fire the update statement when a register already exists in the database. Defaults to false</param>
        /// <param name="validateData"></param>
        /// <returns>true if object is saved on database, otherwise false</returns>
        public bool SaveChanges(bool ignoreAutoIncrementAttribute = true, FireEvent fireEvent = FireEvent.OnBeforeAndAfter, bool doNotUpdateWhenExists = false, bool validateData = false)
        {
#if !PCL
            var _table = TableDefinition.GetTableDefinition(typeof (T));
            var _function = _table.DefaultDataFunction;

            if (validateData)
            {
                if (!this.OnValidateData()) return false;
            }

            if (fireEvent == FireEvent.OnBeforeAndAfter || fireEvent == FireEvent.OnBeforeSave)
                this.OnBeforeSaveData(new NotifyRecordChangesEventArgs(0, SqlStatementsTypes.UnknownStatement));

            var _ret = _function.SaveChanges(this, ignoreAutoIncrementAttribute, doNotUpdateWhenExists);

            ThrowException(_ret);

            if (_ret.ReturnStatus == CrudStatus.Ok && _ret.ChangeType == SqlStatementsTypes.Insert)
            {
                this.SetSelfColumnsIds(_ret.ReturnData);
            }

            if (fireEvent == FireEvent.OnBeforeAndAfter || fireEvent == FireEvent.OnAfterSave)
                this.OnAfterSaveData(new NotifyRecordChangesEventArgs(_ret.RecordsAffected, _ret.ChangeType));

            return _ret.ReturnStatus == CrudStatus.Ok;

#else
            return false;
#endif
        }

        public async Task<int> InsertAsync(bool ignoreAutoIncrementField = true, bool fireOnAfterSaveData = true, bool validateData = false)
        {
            return await Task.Run(() => this.Insert(ignoreAutoIncrementField, fireOnAfterSaveData, validateData));
        }

        public int Insert(bool ignoreAutoIncrementField = true, bool fireOnAfterSaveData = true, bool validateData = false)
        {
#if !PCL
            var _table = TableDefinition.GetTableDefinition(typeof (T));
            var _function = _table.DefaultDataFunction;

            if (validateData)
            {
                if (!this.OnValidateData()) return 0;
            }

            var _ret = _function.Insert(this, ignoreAutoIncrementField);

            ThrowException(_ret);

            if (_ret.ReturnStatus == CrudStatus.Ok && _ret.ChangeType == SqlStatementsTypes.Insert)
            {
                this.SetSelfColumnsIds(_ret.ReturnData);
            }

            if (fireOnAfterSaveData)
                this.OnAfterSaveData(new NotifyRecordChangesEventArgs(_ret.RecordsAffected, _ret.ChangeType));

            return _ret.RecordsAffected;

#else
            return -1;
#endif
        }

        public async Task<int> UpdateAsync(bool fireOnAfterSaveData = true, bool validateData = false)
        {
            return await Task.Run(() => this.Update(fireOnAfterSaveData, validateData));
        }

        public int Update(bool fireOnAfterSaveData = true, bool validateData = false)
        {
#if !PCL
            var _table = TableDefinition.GetTableDefinition(typeof (T));
            var _function = _table.DefaultDataFunction;

            if (validateData)
            {
                if (!this.OnValidateData()) return 0;
            }

            var _ret = _function.Update(this);

            ThrowException(_ret);

            if (fireOnAfterSaveData)
                this.OnAfterSaveData(new NotifyRecordChangesEventArgs(_ret.RecordsAffected, _ret.ChangeType));

            return _ret.RecordsAffected;

#else
            return 0;
#endif
        }

        public async Task<bool> DeleteAsync(bool fireBeforeDeleteDataEvent = true, bool fireAfterDeleteDataEvent = true) //, IDbConnection connection = null, bool autoCommit = true)
        {
            return await Task.Run(() => this.Delete(fireBeforeDeleteDataEvent, fireAfterDeleteDataEvent));
        }

        public bool Delete(bool fireBeforeDeleteDataEvent = true, bool fireAfterDeleteDataEvent = true) //, IDbConnection connection = null, bool autoCommit = true)
        {
#if !PCL
            var _table = TableDefinition.GetTableDefinition(typeof (T));
            var _function = _table.DefaultDataFunction;

            if (fireBeforeDeleteDataEvent)
                this.OnBeforeDeleteData(new NotifyRecordChangesEventArgs(0, SqlStatementsTypes.Delete));

            var _ret = _function.Delete(this);

            ThrowException(_ret);

            if (fireAfterDeleteDataEvent)
                this.OnAfterDeleteData(new NotifyRecordChangesEventArgs(_ret.RecordsAffected, _ret.ChangeType));

            return _ret.RecordsAffected > 0;

#else
            return true;
#endif
        }

        public static async Task<int> DeleteAllAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.Run(() => DeleteAll(predicate));
        }

        public static int DeleteAll(Expression<Func<T, bool>> predicate = null)
        {
#if !PCL
            var _table = TableDefinition.GetTableDefinition(typeof (T));
            var _command = GetDboCommand(predicate, false, SqlStatementsTypes.DeleteAll);
            var _ret = _table.DefaultDataFunction.DeleteAll<T>(_command);

            return _ret.RecordsAffected;
#else
            return -1;
#endif
        }

        public static void Truncate(bool useDropAndCreate = false)
        {
#if !PCL
            var _table = TableDefinition.GetTableDefinition(typeof(T));
            var _function = _table.DefaultDataFunction;

            var _ret = _function.ExecuteStatement(_table.GetScriptDropTable());
            ThrowException(_ret);

            _ret = _function.ExecuteStatement(_table.GetScriptCreateTable());
            ThrowException(_ret);
#endif
        }

        public async Task<bool> ExistsAsync()
        {
            return await Task.Run(() => this.Exists());
        }

        public bool Exists()
        {
#if !PCL
            var _table = TableDefinition.GetTableDefinition(typeof(T));
            var _sql = _table.GetSqlSelectForCheck();

            return _table.DefaultDataFunction.Exists(_sql, this);
#else
            throw new Exception("Código executado via PCL!");
#endif
        }

        public static async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => Exists(predicate));
        }

        public static bool Exists(Expression<Func<T, bool>> predicate)
        {
#if !PCL
            var _table = TableDefinition.GetTableDefinition(typeof(T));
            var _sql = "select count(0) as tt from " + ObjectName;
            var _sqlCommand = GetCommandSelect(_sql, predicate);

            return _table.DefaultDataFunction.Exists(_sqlCommand);
#else
            throw new Exception("Código executado via PCL!");
#endif
        }

        public void SetObjectFieldValue(string fieldName, object value)
        {
#if !PCL
#if WINDOWS_PHONE_APP
            var _prop = this.GetType().GetRuntimeProperties().FirstOrDefault(f => f.Name == fieldName);
#else
            var _prop = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(f => f.Name == fieldName);
#endif
            if (_prop != null && _prop.CanWrite)
                _prop.SetValue(this, value, null);
#endif
        }

        protected void OnErrorOnSaveOrDelete(Exception ex)
        {
            if (this.ErrorOnSaveOrDelete != null)
                this.ErrorOnSaveOrDelete(this, new ErrorOnSaveOrDeleteEventArgs(ex));
        }

        private static IEnumerable<FieldDefAttribute> GetObjectFields()
        {
#if !PCL
            return TableDefinition.GetTableDefinition(typeof(T)).Fields.Select(f => f.Value);
#else
            return new List<FieldDefAttribute>();
#endif
        }

#region "SQL Translator"

        private static DboCommand GetDboCommand(Expression<Func<T, bool>> predicate = null, bool useFieldNames = true, SqlStatementsTypes sqlType = SqlStatementsTypes.Select, object obj  = null)
        {

            var _tableDef = TableDefinition.GetTableDefinition(typeof(T));
            var _selectFields = _tableDef.GetStatementSelect(sqlType == SqlStatementsTypes.SelectReload);

            DboCommand _sqlCommand = null;

            switch (sqlType)
            {
                case SqlStatementsTypes.Select:
                    _sqlCommand = GetCommandSelect(_selectFields, predicate, _tableDef);
                    break;

                case SqlStatementsTypes.DeleteAll:
                    _sqlCommand = GetCommandDelete(predicate);
                    break;

                case SqlStatementsTypes.SelectReload:
                    _sqlCommand = GetCommandSelectReloadMe(_selectFields, obj);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return _sqlCommand;

        }

        public static async Task<IEnumerable<T>> QueryAsync(string sqlStatement, object dynamicParameters = null)
        {
            return await Task.Run(() => Query(sqlStatement, dynamicParameters));
        }

        public static IEnumerable<T> Query(string sqlStatement, object dynamicParameters = null)
        {
#if !PCL
            var _function = TableDefinition.GetTableDefinition(typeof (T)).DefaultDataFunction;
            string _parameters = null;
            if (dynamicParameters != null)
            {
                _parameters = JsonConvert.SerializeObject(dynamicParameters, GetJsonSerializerSettings());
            }
            var _ret = _function.Query<T>(sqlStatement, _parameters);
            //var _ret = TableDefinition.GetTableDefinition(typeof(T)).DefaultDataFunction.Query<T>(sqlStatement, dynamicParameters);
            return _ret;
#else
            throw new Exception("Código executado via PCL!");
#endif
        }

        public static async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.Run(() => Query(predicate));
        }

        public static IEnumerable<T> Query(Expression<Func<T, bool>> predicate = null)
        {

#if !PCL
            const bool useFieldNames = true;
            var _command = GetDboCommand(predicate, useFieldNames);
            var _ret = TableDefinition.GetTableDefinition(typeof(T)).DefaultDataFunction.Query<T>(_command, useFieldNames);
            return _ret;
#else
                throw new Exception("Código executado via PCL!");
#endif
        }

        public async Task<T> ReloadMeAsync()
        {
            return await Task.Run(() => this.ReloadMe());
        }

        public T ReloadMe()
        {
#if !PCL
            var _command = GetDboCommand(null, true, SqlStatementsTypes.SelectReload, this);
            var _ret = TableDefinition.GetTableDefinition(typeof(T)).DefaultDataFunction.ReloadMe<T>(_command);
            return _ret;
#else
            return default(T);
#endif
        }

        public static async Task<T> LoadAsync(string sql, object dynamicParameters = null)
        {
            return await Task.Run(() => Load(sql, dynamicParameters));
        }

        public static T Load(string sql, object dynamicParameters = null)
        {
            return Query(sql, dynamicParameters).FirstOrDefault();
        }

        public static async Task<T> LoadAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => Load(predicate));
        }

        public static T Load(Expression<Func<T, bool>> predicate)
        {
            return Query(predicate).FirstOrDefault();
        }

        public static string GetStatementTrace(Expression<Func<T, bool>> predicate = null)
        {

#if !PCL
            const bool useFieldNames = true;
            return GetDboCommand(predicate, useFieldNames).GetCommandDefinition().CommandText;
#else
                throw new Exception("Código executado via PCL!");
#endif
        }

        private static DboCommand GetCommandSelect(string selectionList, Expression predicate, TableDefinition tabledef = null)
        {
            var _cmdText = selectionList;

            var _where = predicate;
            var _argNames = new List<string>();
            var _argValues = new List<object>();

            CompileResult _w = null;

            if (_where != null)
                _w = CompileResult.CompileExpr(_where, _argNames, _argValues, TableDefinition.GetTableDefinition(typeof(T)).DefaultDataFunction.PrefixParameter, ObjectName);

            if (_w != null)
            {
                if (tabledef != null)
                    _cmdText += "\r\nwhere " + tabledef.ReplaceTableAlias(_w.CommandText, tabledef);
                else
                    _cmdText += "\r\nwhere " + _w.CommandText;
            }

            return new DboCommand(ObjectName, _cmdText, _argNames.ToArray(), _argValues.ToArray());
        }

        private static DboCommand GetCommandSelectReloadMe(string selectionList, object obj)
        {
            return new DboCommand(ObjectName, selectionList, null, null, obj);
        }

        private static DboCommand GetCommandDelete(Expression predicate)
        {
            var _cmdText = "delete from " + ObjectName + " ";

            var _where = predicate;
            var _argNames = new List<string>();
            var _argValues = new List<object>();

            CompileResult _w = null;

            if (_where != null)
                _w = CompileResult.CompileExpr(_where, _argNames, _argValues, TableDefinition.GetTableDefinition(typeof(T)).DefaultDataFunction.PrefixParameter, ObjectName);

            if (_w != null)
                _cmdText += " where " + _w.CommandText;

            return new DboCommand(ObjectName, _cmdText, _argNames.ToArray(), _argValues.ToArray());
        }

#endregion
    }

    public interface IDboCommand
    {
        string SqlStatement { get; set; }
        string[] FieldNames { get; set; }
        object[] FieldValues { get; set; }
        object Obj { get; set; }
    }

#if !PCL && !WINDOWS_PHONE_APP
    [Serializable]
#endif
    public class DboCommand : IDboCommand
    {
        private string m_ObjectName;
        private string m_SqlStatement;
        private string[] m_FieldNames;
        private object[] m_FieldValues;
        private object m_Obj;

        public DboCommand(string objectName, string sqlStatement, string[] fieldNames, object[] fieldValues, object obj = null)
        {
            m_ObjectName = objectName;
            m_SqlStatement = sqlStatement;
            m_FieldNames = fieldNames;
            m_FieldValues = fieldValues;
            m_Obj = obj;
        }

        public string SqlStatement
        {
            get { return m_SqlStatement; }
            set { m_SqlStatement = value; }
        }

        public string[] FieldNames
        {
            get { return m_FieldNames; }
            set { m_FieldNames = value; }
        }

        public object[] FieldValues
        {
            get { return m_FieldValues; }
            set { m_FieldValues = value; }
        }

        public object Obj
        {
            get { return m_Obj; }
            set { m_Obj = value; }
        }

#if !PCL

        public CommandDefinition GetCommandDefinition()
        {
            return m_Obj == null ? new CommandDefinition(SqlStatement, this.GetDynamicParameters()) : new CommandDefinition(SqlStatement, m_Obj);
        }

        private DynamicParameters GetDynamicParameters()
        {
            if (FieldNames.Length == 0) return null;

            var _parameters = new DynamicParameters();
            for (int _index = 0; _index < FieldNames.Length; _index++)
            {
                _parameters.Add(FieldNames[_index].Replace(m_ObjectName + ".", ""), FieldValues[_index]);
            }
            return _parameters;
        }
#endif
    }
}
