using System;

namespace Rednet.DataAccess
{
    public delegate void ErrorOnSaveOrDeleteEventHandler(object sender, ErrorOnSaveOrDeleteEventArgs e);

    public class ErrorOnSaveOrDeleteEventArgs : EventArgs
    {
        private readonly Exception m_Exception = null;

        public ErrorOnSaveOrDeleteEventArgs(Exception exception)
        {
            m_Exception = exception;
        }

        public Exception Exception
        {
            get { return m_Exception; }
        }
    }
}