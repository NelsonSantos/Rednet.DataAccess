using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace Community.CsharpSqlite
{
    public class IsolatedStorageIO
    {
        
		#region Singleton Pattern
			
		private static IsolatedStorageFile _Default;

		///<summary>
		/// Get the default instance of IsolatedStorageFile
		///</summary>
		public static IsolatedStorageFile Default
		{
			get
			{
				if ( _Default == null )
				{
					_Default = IsolatedStorageFile.GetUserStoreForApplication();
				}
				return _Default;
			}
		}
		
		#endregion
			


    }
}
