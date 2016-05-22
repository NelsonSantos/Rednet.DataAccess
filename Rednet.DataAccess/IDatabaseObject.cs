using System;
using System.Collections.Generic;
using System.ComponentModel;

#if !PCL
using System.Data;
using System.Data.Common;
#endif

namespace Rednet.DataAccess
{
    public enum FireEvent
    {
        None,
        OnBeforeSave,
        OnAfterSave,
        OnBeforeAndAfter
    }

    public interface IDatabaseObject
    {

        event ErrorOnSaveOrDeleteEventHandler ErrorOnSaveOrDelete;
        event ErrorOnValidateDataEventHandler ErrorOnValidateData;

        string ToJson(bool compressString = false);
        Dictionary<string, object> ToDictionary();
        void SetIdFields();

        /// <summary>
        /// Save current changes on object
        /// </summary>
        /// <param name="ignoreAutoIncrementAttribute">if true, doesn't include autoincrement fields on insert statement. Defaults to true</param>
        /// <param name="fireEvent">if true, fire the AfterSaveData event. Defaults to true</param>
        /// <param name="doNotUpdateWhenExists">if true, doesn't fire the update statement when a register already exists in the database. Defaults to false</param>
        //// <param name="connection"></param>
        //// <param name="autoCommit">if true, commit pendents transaction on object, otherwise you have to call Commit() method to confirm changes or Rollback() to discard changes. Defaults to true</param>
        /// <returns>true if object is saved on database, otherwise false</returns>
        //bool SaveChanges(bool ignoreAutoIncrementAttribute = true, bool fireOnAfterSaveData = true, bool doNotUpdateWhenExists = false, IDbConnection connection = null, bool autoCommit = true);
        bool SaveChanges(bool ignoreAutoIncrementAttribute = true, FireEvent fireEvent = FireEvent.OnBeforeAndAfter, bool doNotUpdateWhenExists = false, bool validateData = true);
        int Insert(bool ignoreAutoIncrementField = true, bool fireOnAfterSaveData = true, bool validateData = false);
        int Update(bool fireOnAfterSaveData = true, bool validateData = false);
        bool Delete(bool fireBeforeDeleteDataEvent = true, bool fireAfterDeleteDataEvent = true);//, IDbConnection connection = null, bool autoCommit = true);
        //bool Delete(bool fireBeforeDeleteDataEvent = true, bool fireAfterDeleteDataEvent = true, IDbConnection connection = null, bool autoCommit = true);

        void SetObjectFieldValue(string fieldName, object value);
        //bool Commit();

        //bool Rollback();

        bool Exists();
        
        string GetCreateTableScript();
        
        string GetDropTableScript();
        
        string Name { get; }

        IEnumerable<FieldDefAttribute> Fields { get; }
    }
}