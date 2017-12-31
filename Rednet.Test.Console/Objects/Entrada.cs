using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rednet.DataAccess;

namespace Rednet.Test.Console.Objects
{
    public class Entrada : DatabaseObject<Entrada>
    {

        [FieldDef(AutomaticValue = AutomaticValue.AutoIncrement, IsPrimaryKey = true)]
        public int IdEntrada { get; set; }

        //[FieldDef(IgnoreForSave = true)]
        //public string Nome
        //{
        //    get { return this.Visitante?.Nome ?? "Visitante não encontrado"; }
        //}

        //[FieldDef(IgnoreForSave = true)]
        //public long Cpf
        //{
        //    get { return this.Visitante?.Cpf ?? 0; }
        //}

        //[FieldDef(IgnoreForSave = true)]
        //public long Telefone
        //{
        //    get { return this.Visitante?.Telefone ?? 0; }
        //}
        public int IdImovel { get; set; }
        public int IdVisitante { get; set; }
        public DateTime DataHoraEntrada { get; set; } = DateTime.Now;
        public DateTime? DataHoraSaida { get; set; }
        [FieldRule(CheckType = FieldCheck.NullOrEmpty, ValidationText = "O campo 'Autorizado por' não pode ser vazio!")]
        public string Autorizador { get; set; }
        public MotivoVisita MotivoVisita { get; set; }
        [FieldDef(Lenght = 500)]
        public string Observacoes { get; set; }

        //private Imovel m_Imovel;
        //private Visitante m_Visitante;

        //[FieldDef(IgnoreForSave = true)]
        //public Imovel Imovel //{ get; set; }
        //{
        //    get
        //    {
        //        if (m_Imovel != null) return m_Imovel;

        //        m_Imovel = Imovel.Load(i => i.IdImovel == this.IdImovel);

        //        return m_Imovel;
        //    }
        //}

        public StatusEntrada StatusEntrada { get; set; } = StatusEntrada.None;

        //[FieldDef(IgnoreForSave = true, DisplayOnForm = false)]
        //public string SearchField
        //{
        //    get { return $"{this.Nome} {this.Autorizador} {this.Imovel.EnderecoImovel} {this.Cpf} {this.DataHoraEntrada.ToStringBrasil()} {this.DataHoraSaida.ToValue().ToStringBrasil()}"; }
        //}

        [JoinField(JoinRelation = JoinRelation.OneToOne, 
            JoinType = JoinType.LeftJoin, 
            SourceColumnNames = new[] { "IdVisitante" }, 
            TargetColumnNames = new[] { "IdVisitante" }, 
            FilterColumnNames = new[] { "TipoVisitante" }, 
            FilterColumnValues = new object[] { TipoVisitante.Pessoal })]
        public Visitante Visitante { get; set; }

    }

    public enum MotivoVisita
    {
        Pessoal,
        Funcionario,
        PrestacaoServicos,
        Outros
    }

    public enum StatusEntrada
    {
        None,
        Entrada,
        Finalizado,
        Abandonado
    }
}
