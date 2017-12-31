using System;
using System.IO;
using Rednet.DataAccess;

namespace Rednet.Test.Console.Objects
{
    public class Visitante : DatabaseObject<Visitante>
    {
        [FieldDef(AutomaticValue = AutomaticValue.AutoIncrement, IsPrimaryKey = true, Label = "Código", EditOnForm = false)]
        public int IdVisitante { get; set; }

        [FieldRule(CheckType = FieldCheck.NullOrEmpty, ValidationText = "O campo 'Nome' não pode ser vazio!")]
        [FieldDef(Label = "Nome")]
        public string Nome { get; set; }

        [FieldRule(CheckType = FieldCheck.NotEqual, FirstValueToCheck = 0, ValidationText = "O campo 'CPF' não pode ser vazio!")]
        [FieldDef(Label = "CPF", NumberFormat = @"000\.000\.000\-00", ValidChars = "0123456789.-", Lenght = 11)]
        public long Cpf { get; set; }

        [FieldRule(CheckType = FieldCheck.NotEqual, FirstValueToCheck = 0, ValidationText = "O campo 'Telefone' não pode ser vazio!")]
        [FieldDef(Label = "Telefone", NumberFormat = @"{11:\(00\) 00000\-0000};{10:\(00\) 0000\-0000};{9:00000\-0000};{8:0000\-0000}", ValidChars = "0123456789 ()-", Lenght = 11)]
        public long Telefone { get; set; }

        [FieldDef(Label = "Data do Cadastro", EditOnForm = false)]
        public DateTime DatCadastro { get; set; } = DateTime.Now;

        public TipoVisitante TipoVisitante { get; set; }

        [FieldDef(IgnoreForSave = true, DisplayOnForm = false)]
        public string SearchField
        {
            get { return $"{this.Nome} {this.Cpf} {this.Telefone}"; }
        }

        [FieldDef(IgnoreForSave = true, DisplayOnForm = false)]
        public string Editar
        {
            get { return $"Editar"; }
        }

        [FieldDef(IgnoreForSave = true, DisplayOnForm = false)]
        public string Excluir
        {
            get { return $"Excluir"; }
        }

        [FieldDef(IgnoreForSave = true, DisplayOnForm = false)]
        public string Foto
        {
            get { return $"Foto"; }
        }

        [FieldDef(IgnoreForSave = true)]
        public string PathImage
        {
            get { return GetPathImage(this.IdVisitante); }
        }

        public static string GetPathImage(int idVisitante)
        {
            var _idvisitante = idVisitante.ZeroEsquerda(6);
            var _ret = @".\imagens";


            if (!Directory.Exists(_ret))
                Directory.CreateDirectory(_ret);

            _ret = _ret + $@"\visitante_{_idvisitante}.jpg";

            return _ret;
        }

        protected override bool OnValidateData()
        {
            if (!base.OnValidateData()) return false;

            if (File.Exists(this.PathImage)) return true;

            this.RaiseErrorOnValidateData(new ErrorOnValidateDataEventArgs(new ValidatedField[] { new ValidatedField() { FieldMessage = "Não há uma imagem definida para este visitante", FieldName = "PathImage" } }));

            return false;
        }
    }

    public enum TipoVisitante
    {
        Pessoal,
        Servicos
    }
}