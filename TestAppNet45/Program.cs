using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rednet.DataAccess;

namespace TestAppNet45
{

    public class temp_nelson : DatabaseObject<temp_nelson>
    {

        [FieldDef(IsPrimaryKey = true, AutomaticValue = AutomaticValue.AutoIncrement)]
        public int Codigo { get; set; }
        [FieldDef(IsNullAble = true)]
        public string Nome { get; set; }
        [FieldDef(IsNullAble = true)]
        public string SobreNome { get; set; }
        [FieldDef(IsNullAble = true)]
        public string Cidade { get; set; }
        [FieldDef(IsNullAble = true)]
        public string Estado { get; set; }
        [FieldDef(IsNullAble = true)]
        public bool? Ativo { get; set; }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InitDatabase();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static string InitDatabase()
        {

            try
            {
                if (DatabaseObjectShared.DataFunction.Count == 0)
                {
                    var _mainDatabase = "EsMarca";
                    var _mainFunc = GetDataFunction();

                    DatabaseObjectShared.DataFunction.Clear();
                    DatabaseObjectShared.DatabaseName = _mainDatabase;
                    DatabaseObjectShared.DataFunction.Add(_mainDatabase, _mainFunc);

                }

                return "#ok#";

            }
            catch (Exception ex)
            {
                return string.Format("#fail# - {0}\r\n\r\n{1}", ex.Message, ex.StackTrace);
            }

            //var _path = Path.Combine(GetAppDir(), "Scripts");

            //VerifyObject<Cliente>(_mainFunc, _path);


            //var _ws = new  WebServices.EsMarcaServicesClient(new BasicHttpBinding(BasicHttpSecurityMode.None), new EndpointAddress("http://192.168.0.105/esmarca.web/esmarcaservices.svc/teste"));

        }

        public static IDataFunctions GetDataFunction()
        {
            try
            {

                var _ret = new DataFunctionsMySql
                {
                    Server = "mysql01.pdv5.hospedagemdesites.ws", 
                    DataBase = "pdv5", 
                    UserId = "pdv5", 
                    Password = "pdv2015", 
                    Name = "EsMarca",
                    Pooling = true,
                    IncludeSecurityAsserts = true
                };

                return _ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
