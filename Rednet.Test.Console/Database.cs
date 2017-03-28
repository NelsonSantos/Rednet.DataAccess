using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rednet.DataAccess;

namespace Rednet.Test.Console
{
    public static class Database
    {
        private static IDataFunctions m_DataFunction;

        public static IDataFunctions DataFunction
        {
            get { return m_DataFunction; }
        }

        public static void InitDatabase()
        {

            const string databaseName = "main";

            m_DataFunction = GetDataFunction(databaseName);

            DatabaseObjectShared.DataFunctions.Clear();
            DatabaseObjectShared.DefaultDataFunctionName = databaseName;
            DatabaseObjectShared.DataFunctions.Add(databaseName, m_DataFunction);

            //Teste.DeleteAll();
            //for (var i = 0; i < 30; i++)
            //{
            //    var _teste = new Teste {DataHoraSaida = DateTime.Now, IdImovel = 1};
            //    _teste.SaveChanges();
            //}
            //var _watch = new Stopwatch();
            //_watch.Start();
            //var _list = Teste.Query();

            //_watch.Stop();

            //var _a = 10;

        }

        public static void VerifyObject<TDatabaseObject>() where TDatabaseObject : IDatabaseObject
        {
            var _path = SharedFunctions.GetScriptsDir();

            if (m_DataFunction.CheckDdlScript<TDatabaseObject>(_path)) return;
            if (m_DataFunction.AlterTable<TDatabaseObject>()) m_DataFunction.SaveDdlScript<TDatabaseObject>(_path);

        }

        private static IDataFunctions GetDataFunction(string databaseName)
        {
            try
            {
                var _file = SharedFunctions.GetDatabaseFile();

                if (!File.Exists(_file))
                {
                    SQLiteConnection.CreateFile(_file);
                }

                var _ret = new DataFunctionsSQLite() { DatabaseFile = _file, Name = databaseName };

                return _ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
