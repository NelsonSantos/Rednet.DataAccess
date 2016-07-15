using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
////using System.Windows.Controls;
////using System.Windows.Documents;
////using System.Windows.Ink;
using System.Windows.Input;
////using System.Windows.Media;
////using System.Windows.Media.Animation;
////using System.Windows.Shapes;

namespace System.Data
{

    public sealed class DataColumnCollection : Dictionary<String, Type>
    {
    }

    /// <summary>
    /// Represents a table of data. 
    /// This is primary an IEnumerable{Row} collection. 
    /// The table may be just read-only streaming over the rows, which is ideal for large files of millions of rows. 
    /// Or it may have loaded the entire table into memory, which can be ideal for mutation. 
    /// </summary>
    public class DataTable
    {

        internal readonly DataColumnCollection columnCollection;
        private List<object[]> m_Rows = new List<object[]>();

        public DataColumnCollection Columns
        {
            get { return columnCollection; }
        }

        private void ResetColumns()
        {
            // this method is used design-time scenarios via reflection
            //   by the property grid context menu to show the Reset option or not
            Columns.Clear();
        }

        public List<object[]> Rows
        {
            get { return m_Rows; }
            set { m_Rows = value; }
        }
    }
}
