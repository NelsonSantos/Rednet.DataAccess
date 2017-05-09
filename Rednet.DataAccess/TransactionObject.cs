using System;
#if !PCL
using System.Data;
using System.Data.Common;
#endif

namespace Rednet.DataAccess
{
    public class TransactionObject : IDisposable
    {
        public TransactionObject(bool autoCommit = false)
        {
            this.AutoCommit = autoCommit;
        }
        public bool AutoCommit { get; }
#if !PCL
        private IDbConnection m_Connection = null;
        private IDbTransaction m_Transaction = null;

        internal void SetConnection(IDbConnection connection)
        {
            try
            {
                m_Connection = connection;
                if (m_Connection.State != ConnectionState.Open)
                    m_Connection.Open();

                m_Transaction = m_Connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public IDbTransaction Transaction
        {
            get { return m_Transaction; }
        }

        public IDbConnection Connection
        {
            get { return m_Connection; }
        }
#endif

        public void Commit()
        {
#if !PCL
            m_Transaction.Commit();
            m_Connection.Close();
#endif
        }

        public void Rollback()
        {
#if !PCL
            m_Transaction.Rollback();
            m_Connection.Close();
#endif
        }
        public void Dispose()
        {
#if !PCL
            m_Transaction.Dispose();
            m_Connection.Dispose();
            m_Connection = null;
            m_Transaction = null;
#endif
        }
    }
}