using Rednet.DataAccess;

namespace Rednet.Test.Console.Objects
{
    public class Imovel : DatabaseObject<Imovel>
    {
        [FieldDef(AutomaticValue = AutomaticValue.AutoIncrement, IsPrimaryKey = true, Label = "Código", DisplayOnForm = false)]
        public int IdImovel { get; set; }
        [FieldDef(Label = "Apartamento", Lenght = 20)]
        public string EnderecoImovel { get; set; }
        [FieldDef(Label = "Proprietário", Lenght = 40)]
        public string Proprietario { get; set; }
        [FieldDef(Label = "Telefone", NumberFormat = @"{11:\(00\) 00000\-0000};{10:\(00\) 0000\-0000};{9:00000\-0000};{8:0000\-0000}", ValidChars = "0123456789 ()-", Lenght = 11)]
        public long Telefone { get; set; }
        [FieldDef(Label = "Observações", DisplayOnGrid = false)]
        public string Observacoes { get; set; }
    }
}