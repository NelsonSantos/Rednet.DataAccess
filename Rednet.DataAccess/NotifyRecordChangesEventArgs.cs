using System;
#if !PCL
using System.Data;
using System.Data.Common;
#endif

namespace Rednet.DataAccess
{
    public class NotifyRecordChangesEventArgs : EventArgs
    {

        /// <summary>
        /// Indica qual o type de instrução DML foi executado no banco de dados
        /// </summary>
        public SqlStatementsTypes ChangeType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TransactionObject Transaction { get; private set; }

        /// <summary>
        /// Indica a quantidade de registros afetados pelo comando correspondente
        /// </summary>
        public int RecordsAffected { get; private set; }

#if !PCL
        public NotifyRecordChangesEventArgs(int recordsAffected, SqlStatementsTypes changeType, TransactionObject transaction)
        {
            ChangeType = changeType;
            Transaction = transaction;
            RecordsAffected = recordsAffected;
        }
#endif
    }
}