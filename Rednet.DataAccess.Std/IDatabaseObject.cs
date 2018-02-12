using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

#if !PCL
using System.Data;
using System.Data.Common;
#endif

namespace Rednet.DataAccess
{

    public interface IDatabaseObject
    {

        event ErrorOnSaveOrDeleteEventHandler ErrorOnSaveOrDelete;
        event ErrorOnValidateDataEventHandler ErrorOnValidateData;

        Task<string> ToJsonAsync(bool compressString = false);
        string ToJson(bool compressString = false);
        Task<Dictionary<string, object>> ToDictionaryAsync();
        Dictionary<string, object> ToDictionary();
        void SetIdFields();
        Task<bool> SaveChangesAsync(bool ignoreAutoIncrementAttribute = true, FireEvent fireEvent = FireEvent.OnBeforeAndAfter, bool doNotUpdateWhenExists = false, bool validateData = true, TransactionObject transaction = null);
        bool SaveChanges(bool ignoreAutoIncrementAttribute = true, FireEvent fireEvent = FireEvent.OnBeforeAndAfter, bool doNotUpdateWhenExists = false, bool validateData = true, TransactionObject transaction = null);
        Task<int> InsertAsync(bool ignoreAutoIncrementField = true, bool fireOnAfterSaveData = true, bool validateData = false, TransactionObject transaction = null);
        int Insert(bool ignoreAutoIncrementField = true, bool fireOnAfterSaveData = true, bool validateData = false, TransactionObject transaction = null);
        Task<int> UpdateAsync(bool fireOnAfterSaveData = true, bool validateData = false, TransactionObject transaction = null);
        int Update(bool fireOnAfterSaveData = true, bool validateData = false, TransactionObject transaction = null);
        Task<bool> DeleteAsync(bool fireBeforeDeleteDataEvent = true, bool fireAfterDeleteDataEvent = true, TransactionObject transaction = null);
        bool Delete(bool fireBeforeDeleteDataEvent = true, bool fireAfterDeleteDataEvent = true, TransactionObject transaction = null);
        void SetObjectFieldValue(string fieldName, object value);
        Task<bool> ExistsAsync();
        bool Exists();
        string GetCreateTableScript();
        string GetDropTableScript();
        string Name { get; }
        IEnumerable<FieldDefAttribute> Fields { get; }
    }
}