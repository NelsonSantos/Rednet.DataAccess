using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Rednet;
using Rednet.DataAccess;
using Rednet.Shared.GPS;

namespace TestAppNet45
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var _deleted = temp_nelson.DeleteAll();
            //var _saved = false;
            //var _rnd = new Random();

            //for (int i = 0; i < 50; i++)
            //{
            //    var _codigo = (i + 1);
            //    var _cliente = new temp_nelson()
            //    {
            //        Nome = string.Format("Nelson {0}", _codigo),
            //        SobreNome = string.Format("Nelson {0}", _codigo),
            //        Cidade = "Ribeirão Preto",
            //        Estado = "SP",
            //        Ativo = true
            //    };
            //    _saved = _cliente.SaveChanges();
            //}

            var _watch = Stopwatch.StartNew();
            var _aa = 10;
            //if (_saved)
            //{
            //var _parameter = new {Nome = "Nelson 50"};
            var _ret = temp_nelson.Query("select * from temp_nelson ");//"where Nome = @Nome", _parameter);

            //var _ret2 = temp_nelson.Update(c => new { c.Nome, c.Ativo}, () => new {Nome="Teste", Ativo=false}, null);
   
            _aa = 10;
            //}
            _watch.Stop();
            Debug.WriteLine("Tempo de processamento: " + _watch.Elapsed.ToString("g"));
            _aa = 10;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var _geo = new GMapV2Direction();

            try
            {
                var _ret = await _geo.GetLocationFromAddressAsync("Rua Mario de Andrade, 1111, Ribeirão preto-SP");

            }
            catch (Exception ex)
            {
                var _a = 10;
            }
        }
    }

    public class Teste : psq_base
    {
        public string Campo1 { get; set; }
    }

    public class clientes : DatabaseObject<clientes>
    {
        [FieldDef(IsPrimaryKey = true, AutomaticValue = AutomaticValue.AutoIncrement)]
        public int id_cliente { get; set; }
        public string nome_cliente { get; set; }
        public string razao_social_cli { get; set; }
        public int? meta_promotores { get; set; }
        public int? meta_lojas { get; set; }
        public int? meta_estados { get; set; }
        public int? meta_municipios { get; set; }
        public string cnpj_cli { get; set; }
        public bool ativo { get; set; }

    }

    public class psq_base : DatabaseObject<psq_base>
    {
        [FieldDef(IsPrimaryKey = true, AutomaticValue = AutomaticValue.AutoIncrement)]
        public int id_pesq { get; set; }
        public int? id_psq_cli { get; set; }
        public int? id_psq_prodcli { get; set; }
        public int? psq_regiao { get; set; }
        public DateTime data_inic { get; set; }
        public DateTime data_fim { get; set; }
        public string msg { get; set; }
        public bool psq_status { get; set; }

        [JsonIgnore]
        [JoinField(SourceColumnNames = new string[] { "id_psq_cli" }, TargetColumnNames = new string[] { "id_cliente" }, JoinRelation = JoinRelation.OneToOne, JoinType = JoinType.LeftJoin)]
        public clientes Cliente { get; set; }

        [JsonIgnore]
        [FieldDef(IgnoreForSave = true)]
        public string ClienteNome { get { return this.Cliente == null ? "Cliente não encontrado!" : this.Cliente.nome_cliente; } }

        [JsonIgnore]
        [FieldDef(IgnoreForSave = true)]
        public string PesquisaId { get { return string.Format("#{0}", this.id_pesq.ZeroEsquerda(3)); } }

        [JsonIgnore]
        [FieldDef(IgnoreForSave = true)]
        public string Periodo { get { return string.Format("Dê {0} à {1}", this.data_inic.ToStringBrasil(false), this.data_fim.ToStringBrasil(false)); } }
    }
}
