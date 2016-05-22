using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CoreLocation;
using PCLStorage;
using Rednet.DataAccess;

namespace TestAppiOS
{

    public class Cliente : DatabaseObject<Cliente>
    {

        public Cliente() : base()
        {
        }

        [FieldDef(IsPrimaryKey = true)]
        public int Codigo { get; set; }
        [FieldDef(IsNullAble = true)]
        public string Nome { get; set; }
        [FieldDef(IsNullAble = true)]
        public string SobreNome { get; set; }
        [FieldDef(IsNullAble = true)]
        public int? Idade { get; set; }
        [FieldDef(IsNullAble = true)]
        public string Cidade { get; set; }
        [FieldDef(IsNullAble = true)]
        public string Estado { get; set; }
        [FieldDef(IsNullAble = true)]
        public bool? Ativo { get; set; }
    }


    public static class Database
    {
        private static IFolder m_Root = FileSystem.Current.LocalStorage;
        public static void VerifyObject<TDatabaseObject>(IDataFunctions func, string path) where TDatabaseObject : IDatabaseObject
        {
            if (func.CheckDdlScript<TDatabaseObject>(path)) return;
            if (func.AlterTable<TDatabaseObject>()) func.SaveDdlScript<TDatabaseObject>(path);
        }

        public static void InitDatabase()
        {
            var _mainDatabase = "database";
            var _mainFunc = GetDataFunction(_mainDatabase);

            DatabaseObjectShared.DataFunction.Clear();
            DatabaseObjectShared.DatabaseName = _mainDatabase;
            DatabaseObjectShared.DataFunction.Add(_mainDatabase, _mainFunc);

            var _path = Path.Combine(GetAppDir(), "Scripts");

            VerifyObject<Cliente>(_mainFunc, _path);

            var _deleted = Cliente.DeleteAll();
            var _saved = false;
            var _rnd = new Random();

            for (int i = 0; i < 1000; i++)
            {
                var _codigo = (i + 1);
                var _cliente = new Cliente()
                {
                    Codigo = _codigo,
                    Nome = string.Format("Nelson {0}", _codigo),
                    SobreNome = string.Format("Nelson {0}", _codigo),
                    Cidade = "Ribeirão Preto",
                    Estado = "SP",
                    Idade = _rnd.Next(75),
                    Ativo = true
                };
                _saved = _cliente.SaveChanges();
            }

            var _watch = Stopwatch.StartNew();
            var _aa = 10;
            //if (_saved)
            //{
                //var _parameter = new {Nome = "Nelson 50"};
                var _ret = Cliente.Query("select * from cliente ");//"where Nome = @Nome", _parameter);

            //var _ret2 = Cliente.Update(c =>
            //{
            //    c.Codigo = 10;
            //}, c => c.Ativo == true && c.Codigo == 1);
            //    _aa = 10;
            ////}
            //_watch.Stop();
            //Debug.WriteLine("Tempo de processamento: " + _watch.Elapsed.ToString("g"));
            //_aa = 10;

        }

        public static IDataFunctions GetDataFunction(string databaseName)
        {
            try
            {
                var _file = GetDatabaseFile(databaseName);

                if (m_Root.CheckExistsAsync(_file).Result != ExistenceCheckResult.FileExists)
                {
                    m_Root.CreateFileAsync(_file, CreationCollisionOption.FailIfExists);
                }

                var _ret = new DataFunctionsSQLite() { DatabaseFile = _file, Name = "EsMarca" };

                return _ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static bool IsDebugMode
        {
            get
            {
                var _isdebug = false;
#if DEBUG

                _isdebug = true;
#endif
                return _isdebug;
            }
        }

        public static bool CheckDirectoryAndCreate(string path)
        {
            if (m_Root.CheckExistsAsync(path).Result != ExistenceCheckResult.FolderExists)
            {
                m_Root.CreateFolderAsync(path, CreationCollisionOption.FailIfExists);
                return m_Root.CheckExistsAsync(path).Result == ExistenceCheckResult.FolderExists;
            }
            return true;
        }

        public static string GetAppDir()
        {
            //var _pathShared = Path.GetFullPath("/mnt/shared/AndroidSharedFolders/FoodTruck");
            var _pathApp = m_Root.Path;// Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, "FoodTruck"));
            var _path = "";


            //if (IsDebugMode && CheckDirectoryAndCreate(_pathShared))
            //    _path = _pathShared;
            //else if (CheckDirectoryAndCreate(_pathApp))
            CheckDirectoryAndCreate(_pathApp);
            _path = _pathApp;

            return _path;
        }

        public static string GetDatabaseFile(string databaseName)
        {
            var _ret = Path.Combine(GetAppDir(), "Database");

            if (m_Root.CheckExistsAsync(_ret).Result != ExistenceCheckResult.FolderExists)
                m_Root.CreateFolderAsync(_ret, CreationCollisionOption.FailIfExists);

            _ret = Path.Combine(_ret, string.Format("{0}.db3", databaseName));

            return _ret;
        }

    }
}
