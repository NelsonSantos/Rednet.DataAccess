//
// Community.CsharpSqlite.SQLiteClient.SqliteCommand.cs
//
// Represents a Transact-SQL statement or stored procedure to execute against 
// a Sqlite database file.
//
// Author(s): 	Vladimir Vukicevic  <vladimir@pobox.com>
//		Everaldo Canuto  <everaldo_canuto@yahoo.com.br>
//		Chris Turchin <chris@turchin.net>
//		Jeroen Zwartepoorte <jeroen@xs4all.nl>
//		Thomas Zoechling <thomas.zoechling@gmx.at>
//		Joshua Tauberer <tauberer@for.net>
//    Noah Hart <noah.hart@gmail.com>
//
// Copyright (C) 2002  Vladimir Vukicevic
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// FIX
using Hardcodet.Silverlight.Util;

using System;
using System.Text;
using System.Data;
//#if NET_2_0
using System.Data.Common;
using System.Reflection;
using System.Runtime.InteropServices;
//#endif
using Community.CsharpSqlite;


namespace Community.CsharpSqlite.SQLiteClient 
{
#if NET_2_0
	public class SqliteCommand : DbCommand, ICloneable
#else
#if !SILVERLIGHT
    public class SqliteCommand : IDbCommand, ICloneable
#else
    public class SqliteCommand : IDbCommand, IDisposable
#endif
	
#endif
	{
		#region Fields
		
		private IDbConnection parent_conn;
		
        private IDbTransaction transaction;
		
		private string sql;
		private int timeout;
		private CommandType type;
		private UpdateRowSource upd_row_source;
		private IDataParameterCollection sql_params;
		private bool prepared = false;
		private bool _designTimeVisible = true;
		
		#endregion

		#region Constructors and destructors
		
		public SqliteCommand ()
		{
			sql = "";
		}
				
		public SqliteCommand (string sqlText)
		{
			sql = sqlText;
		}
		
		public SqliteCommand (string sqlText, SqliteConnection dbConn)
		{
			sql = sqlText;
			parent_conn = dbConn;
		}


#if SILVERLIGHT
        public SqliteCommand(string sqlText, SqliteConnection dbConn, SqliteTransaction trans)
        {
            sql = sqlText;
            parent_conn = dbConn;
            transaction = trans;
        }
#else
        public SqliteCommand (string sqlText, IDbConnection dbConn, IDbTransaction trans)
		{
			sql = sqlText;
			parent_conn = dbConn;
			transaction = trans;
		}
#endif


#if !NET_2_0
        public void Dispose ()
		{
		}
#endif
		
		#endregion

		#region Properties
		
#if NET_2_0
		override
#endif
		public string CommandText 
		{
			get { return sql; }
            set
            {
                // FIX
                var b = Encoding.UTF8.GetBytes(value);

                ASCII ascii = new ASCII();
                sql = ascii.GetString(b, 0, b.Length);
                prepared = false;
            }

			//set { ASCIIEncoding ascii = new ASCIIEncoding(); sql = ascii.GetString(Encoding.UTF8.GetBytes(value)); prepared = false; }
		}
	
#if NET_2_0
		override
#endif
		public int CommandTimeout
		{
			get { return timeout; }
			set { timeout = value; }
		}
		
#if NET_2_0
		override
#endif
		public CommandType CommandType 
		{
			get { return type; }
			set { type = value; }
		}
		
#if NET_2_0
		protected override DbConnection DbConnection
		{
			get { return parent_conn; }
			set { parent_conn = (SqliteConnection)value; }
		}
#endif
		public IDbConnection Connection
		{
			get { return parent_conn; }
			set { parent_conn = (SqliteConnection)value; }
		}
		
#if !NET_2_0 && !SILVERLIGHT
		IDbConnection IDbCommand.Connection 
		{
			get 
			{ 
				return parent_conn; 
			}
			set 
			{
				if (!(value is SqliteConnection)) 
				{
					throw new InvalidOperationException ("Can't set Connection to something other than a SqliteConnection");
				}
				parent_conn = (SqliteConnection) value;
			}
		}
#endif

		public
#if NET_2_0
		new
#endif
        IDataParameterCollection Parameters 

		{
			get
			{
				if (sql_params == null)
					sql_params = new SqliteParameterCollection();
				return sql_params;
			}
		}

#if NET_2_0
		protected override DbParameterCollection DbParameterCollection
                {
                        get { return (DbParameterCollection) Parameters; }
                }
#endif
		
#if !NET_2_0 && !SILVERLIGHT
		IDataParameterCollection IDbCommand.Parameters 
		{
			get { return Parameters; }
		}
		
#endif


#if SILVERLIGHT

        ///<summary>
        ///Get or set the value of Transaction
        ///</summary>
        public IDbTransaction Transaction
        {
            get
            {
                return transaction;
            }
            set
            {
                transaction = value;
            }
        }
#else
		
#if NET_2_0
		protected override DbTransaction DbTransaction
#else
		public IDbTransaction Transaction 
#endif
		{
			get {
#if NET_2_0
				return (DbTransaction)transaction;
#else
				return transaction;
#endif
			}
			set { transaction = value; }
		}
#endif
		
#if NET_2_0
		public override bool DesignTimeVisible
		{
			get { return _designTimeVisible; }
			set { _designTimeVisible = value; }
		}
#endif

#if NET_2_0
		override
#endif
		public UpdateRowSource UpdatedRowSource 
		{
			get { return upd_row_source; }
			set { upd_row_source = value; }
		}
		                
		#endregion

		#region Internal Methods
		
		internal int NumChanges () 
		{
			//if (parent_conn.Version == 3)
				return Sqlite3.Changes((parent_conn as SqliteConnection).Handle2);
			//else
			//	return Sqlite.sqlite_changes(parent_conn.Handle);
		}
		
		private void BindParameters3 (IntPtr pStmt)
		{
			if (sql_params == null) return;
			if (sql_params.Count == 0) return;
			
			int pcount = Sqlite3.BindParameterCount(pStmt);
		    var _replaces = "@?:$".ToCharArray();

			for (int i = 1; i <= pcount; i++)
			{
			    var name = Sqlite3.BindParameterName(pStmt, i);

			    foreach (var _replace in _replaces)
			    {
                    name = name.Replace(_replace.ToString(), "");
			    }

				SqliteParameter param = null;
				if (name != null)
					param = sql_params[name] as SqliteParameter;
				else
					param = sql_params[i-1] as SqliteParameter;
				
				if (param.Value == null) {
					Sqlite3.BindNull (pStmt, i);
					continue;
				}
					
				Type ptype = param.Value.GetType();
#if WINDOWS_PHONE_APP
                if (ptype.GetTypeInfo().IsEnum)
#else
                if (ptype.IsEnum)
#endif
                    ptype = Enum.GetUnderlyingType (ptype);
				
				SqliteError err;
				
				if (ptype.Equals (typeof (String))) 
				{
					String s = (String)param.Value;
                    err = (SqliteError)Sqlite3.BindText(pStmt, i, s, -1, IntPtr.Zero);
				} 
				else if (ptype.Equals (typeof ( DBNull))) 
				{
                    err = (SqliteError)Sqlite3.BindNull(pStmt, i);
				}
				else if (ptype.Equals (typeof (Boolean))) 
				{
					bool b = (bool)param.Value;
                    err = (SqliteError)Sqlite3.BindInt(pStmt, i, b ? 1 : 0);
				} else if (ptype.Equals (typeof (Byte))) 
				{
                    err = (SqliteError)Sqlite3.BindInt(pStmt, i, (Byte)param.Value);
				}
				else if (ptype.Equals (typeof (Char))) 
				{
                    err = (SqliteError)Sqlite3.BindInt(pStmt, i, (Char)param.Value);
				}
#if WINDOWS_PHONE_APP
                else if (ptype.GetTypeInfo().IsEnum)
#else
                else if (ptype.IsEnum)
#endif
                {
                    err = (SqliteError)Sqlite3.BindInt(pStmt, i, (Int32)param.Value);
				}
				else if (ptype.Equals (typeof (Int16))) 
				{
                    err = (SqliteError)Sqlite3.BindInt(pStmt, i, (Int16)param.Value);
				} 
				else if (ptype.Equals (typeof (Int32))) 
				{
                    err = (SqliteError)Sqlite3.BindInt(pStmt, i, (Int32)param.Value);
				}
				else if (ptype.Equals (typeof (SByte))) 
				{
                    err = (SqliteError)Sqlite3.BindInt(pStmt, i, (SByte)param.Value);
				} 
				else if (ptype.Equals (typeof (UInt16))) 
				{
                    err = (SqliteError)Sqlite3.BindInt(pStmt, i, (UInt16)param.Value);
				}
				else if (ptype.Equals (typeof (DateTime))) 
				{
					DateTime dt = (DateTime)param.Value;
                    err =(SqliteError)Sqlite3.BindText(pStmt,i,dt.ToString("yyyy-MM-dd  HH:mm:ss"),-1,IntPtr.Zero);
        }
        else if (ptype.Equals(typeof(Decimal)))
        {
          err = (SqliteError)Sqlite3.BindDouble(pStmt, i, Decimal.ToDouble((Decimal)param.Value));
        }
        else if (ptype.Equals(typeof(Double))) 
				{
                    err = (SqliteError)Sqlite3.BindDouble(pStmt, i, (Double)param.Value);
				}
				else if (ptype.Equals (typeof (Single))) 
				{
                    err = (SqliteError)Sqlite3.BindDouble(pStmt, i, (Single)param.Value);
				} 
				else if (ptype.Equals (typeof (UInt32))) 
				{
                    err = (SqliteError)Sqlite3.BindInt64(pStmt, i, (UInt32)param.Value);
				}
				else if (ptype.Equals (typeof (Int64))) 
				{
                    err = (SqliteError)Sqlite3.BindInt64(pStmt, i, (Int64)param.Value);
				} 
				else if (ptype.Equals (typeof (Byte[]))) 
				{
                    err = (SqliteError)Sqlite3.BindBlob(pStmt, i, (byte[])param.Value, ((byte[])param.Value).Length, IntPtr.Zero);
				}
        else if (ptype.Equals(typeof(Guid)))
        {
          err = (SqliteError)Sqlite3.BindText(pStmt, i, param.Value.ToString(), param.Value.ToString().Length, IntPtr.Zero);
        }
        else 
				{
					throw new ApplicationException("Unkown Parameter Type");
				}
				if (err != SqliteError.OK) 
				{
					throw new ApplicationException ("Sqlite error in bind " + err);
				}
			}
		}

		//private void GetNextStatement (string pzStart, ref IntPtr pzTail, ref IntPtr pStmt)
		//{
		//    //UTF8Encoding encoding = new UTF8Encoding();

  //          SqliteError err = (SqliteError)Sqlite3.Prepare2((parent_conn as SqliteConnection).Handle2, pzStart, -1 /*pzStart.Length*/, out pStmt, out pzTail);
		//	if (err != SqliteError.OK)
		//		throw new SqliteSyntaxException (GetError3());
		//}


        // Executes a statement and ignores its result.
        private void ExecuteStatement(IntPtr pStmt)
        {
			int cols;
			IntPtr pazValue, pazColName;
			ExecuteStatement (pStmt, out cols, out pazValue, out pazColName);
		}

		// Executes a statement and returns whether there is more data available.
        internal bool ExecuteStatement(IntPtr pStmt, out int cols, out IntPtr pazValue, out IntPtr pazColName)
        {
			SqliteError err;
			
			//if (parent_conn.Version == 3) 
			//{
				err = (SqliteError)Sqlite3.Step (pStmt);
                
				if (err == SqliteError.ERROR)
					throw new SqliteExecutionException (GetError3());
                
				pazValue = IntPtr.Zero; pazColName = IntPtr.Zero; // not used for v=3
				cols = Sqlite3.ColumnCount(pStmt);
			
            /*
            }
			else 
			{
                err = (SqliteError)Sqlite3.Step(pStmt, out cols, out pazValue, out pazColName);
				if (err == SqliteError.ERROR)
					throw new SqliteExecutionException ();
			}
			*/
			if (err == SqliteError.BUSY)
				throw new SqliteBusyException();
			
			if (err == SqliteError.MISUSE)
				throw new SqliteExecutionException();
				
			// err is either ROW or DONE.
			return err == SqliteError.ROW;
		}
		
#endregion

#region Public Methods

#if !SILVERLIGHT

		object ICloneable.Clone ()
		{
			return new SqliteCommand (sql, parent_conn, transaction);
		}

#endif

#if NET_2_0
		override
#endif
		public void Cancel ()
		{
		}
				
#if NET_2_0
		override
#endif
		public void Prepare ()
		{
			// There isn't much we can do here.  If a table schema
			// changes after preparing a statement, Sqlite bails,
			// so we can only compile statements right before we
			// want to run them.
			
			if (prepared) return;			
			prepared = true;
		}
		
#if !NET_2_0 && !SILVERLIGHT
		IDbDataParameter IDbCommand.CreateParameter()
		{
			return CreateParameter ();
		}
#endif
		
#if NET_2_0
		protected override DbParameter CreateDbParameter ()
#else
		public IDbDataParameter CreateParameter ()
#endif
		{
			return new SqliteParameter ();
		}
		
#if NET_2_0
		override
#endif
		public int ExecuteNonQuery ()
		{
			int rows_affected;
			ExecuteReader (CommandBehavior.Default, false, out rows_affected);
			return rows_affected;
		}
		
#if NET_2_0
		override
#endif
		public object ExecuteScalar ()
		{
			SqliteDataReader r = (SqliteDataReader)ExecuteReader ();
			if (r == null || !r.Read ()) {
				return null;
			}
			object o = r[0];
			r.Close ();
			return o;
		}
		
#if !NET_2_0
#if !SILVERLIGHT
		IDataReader IDbCommand.ExecuteReader ()
		{
			return ExecuteReader ();
		}
		
		IDataReader IDbCommand.ExecuteReader (CommandBehavior behavior)
		{
			return ExecuteReader (behavior);
		}
#endif
		
		public IDataReader ExecuteReader ()
		{
			return ExecuteReader (CommandBehavior.Default);
		}
#endif
		
		public IDataReader ExecuteReader (CommandBehavior behavior)
		{
			int r;
			return ExecuteReader (behavior, true, out r);
		}
		
#if NET_2_0
		public new SqliteDataReader ExecuteReader ()
		{
			return ExecuteReader (CommandBehavior.Default);
		}
		
		protected override DbDataReader ExecuteDbDataReader (CommandBehavior behavior)
		{
			return (DbDataReader) ExecuteReader (behavior);
		}
#endif

		public IDataReader ExecuteReader (CommandBehavior behavior, bool want_results, out int rows_affected)
		{
			Prepare ();

            // The SQL string may contain multiple sql commands, so the main
            // thing to do is have Sqlite iterate through the commands.
            // If want_results, only the last command is returned as a
            // DataReader.  Otherwise, no command is returned as a
            // DataReader.

            //IntPtr psql; // pointer to SQL command

            // Sqlite 2 docs say this: By default, SQLite assumes that all data uses a fixed-size 8-bit 
            // character (iso8859).  But if you give the --enable-utf8 option to the configure script, then the 
            // library assumes UTF-8 variable sized characters. This makes a difference for the LIKE and GLOB 
            // operators and the LENGTH() and SUBSTR() functions. The static string sqlite_encoding will be set 
            // to either "UTF-8" or "iso8859" to indicate how the library was compiled. In addition, the sqlite.h 
            // header file will define one of the macros SQLITE_UTF8 or SQLITE_ISO8859, as appropriate.
            // 
            // We have no way of knowing whether Sqlite 2 expects ISO8859 or UTF-8, but ISO8859 seems to be the
            // default.  Therefore, we need to use an ISO8859(-1) compatible encoding, like ANSI.
            // OTOH, the user may want to specify the encoding of the bytes stored in the database, regardless
            // of what Sqlite is treating them as, 

            // For Sqlite 3, we use the UTF-16 prepare function, so we need a UTF-16 string.

            //if (parent_conn.Version == 2)
            //    psql = Sqlite.StringToHeap(sql.Trim(), parent_conn.Encoding);
            //else
            //    var psql = Marshal.StringToHGlobalUni(sql.Trim());

            string queryval = sql.Trim();
            //string pzTail = sql.Trim();
		    //IntPtr pzTail = IntPtr.Zero;
            Sqlite3.Result errMsgPtr;

            (parent_conn as SqliteConnection).StartExec ();

			rows_affected = 0;
			
			try {
				while (true) {
					IntPtr pStmt = IntPtr.Zero;
                    
                    //queryval = pzTail;
				    pStmt = Sqlite3.Prepare2((parent_conn as SqliteConnection).Handle2, ref queryval);
                    //GetNextStatement(queryval, ref pzTail, ref pStmt);
                
			        if (pStmt == null)
					    throw new Exception();

                    // pzTail is positioned after the last byte in the
                    // statement, which will be the NULL character if
                    // this was the last statement.

                    //bool last = pzTail == IntPtr.Zero;
                    bool last = queryval.Length == 0;

                    try
                    {
						if ((parent_conn as SqliteConnection).Version == 3)
							BindParameters3 (pStmt);
						
						if (last && want_results)
							return new SqliteDataReader (this, pStmt, (parent_conn as SqliteConnection).Version);

						ExecuteStatement(pStmt);
						
						if (last) // rows_affected is only used if !want_results
							rows_affected = NumChanges ();
						
					} finally {
						//if (parent_conn.Version == 3) 
							errMsgPtr = Sqlite3.Finalize (pStmt);
						//else
						//	Sqlite.sqlite_finalize (pStmt, out errMsgPtr);
					}
					
					if (last) break;
				}

				return null;
			} finally {
                (parent_conn as SqliteConnection).EndExec ();
				//Marshal.FreeHGlobal (psql); 
			}
		}

		public int LastInsertRowID () 
		{
			return (parent_conn as SqliteConnection).LastInsertRowId;
		}
    public string GetLastError()
    {
      return Sqlite3.GetErrmsg((parent_conn as SqliteConnection).Handle2);
    }
		private string GetError3() {
		    return Sqlite3.GetErrmsg((parent_conn as SqliteConnection).Handle2);
			//return Marshal.PtrToStringUni (Sqlite.sqlite3_errmsg16 (parent_conn.Handle));
		}
#endregion
	}
}
