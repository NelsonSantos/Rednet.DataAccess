using System;
#if !PCL
using System.Data;
using System.Data.Common;
#endif

namespace Rednet.DataAccess
{
    public class NotifyRecordChangesEventArgs : EventArgs
    {

//#if !PCL
//        public IDbConnection CurrentConnection { get; private set; }
//#endif
        /// <summary>
        /// Indica qual o type de instrução DML foi executado no banco de dados
        /// </summary>
        public SqlStatementsTypes ChangeType { get; private set; }

        /// <summary>
        /// Indica a quantidade de registros afetados pelo comando correspondente
        /// </summary>
        public int RecordsAffected { get; private set; }

        ///// <summary>
        ///// Indica se o evento deve ser executado pelo objecto controlador do evento.\r\n\r\nÉ importante lembrar que caso não haja validação dessa propriedade o evento será disparado.
        ///// </summary>
        //public bool FireEvent { get; private set; }

#if !PCL
        public NotifyRecordChangesEventArgs(/*bool fireEvent,*/ int recordsAffected, SqlStatementsTypes changeType/*, IDbConnection currentConnection*/)
        {
            ChangeType = changeType;
            RecordsAffected = recordsAffected;
            //FireEvent = fireEvent;
            //CurrentConnection = currentConnection;
        }
#endif
    }
}