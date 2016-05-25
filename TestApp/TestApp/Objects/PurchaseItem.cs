using Rednet.DataAccess;

namespace TestApp.Objects
{
    public class PurchaseItem : DatabaseObject<PurchaseItem>
    {
        [FieldDef(IsPrimaryKey = true)]
        public int PurchaseId { get; set; }

        [FieldDef(IsPrimaryKey = true)]
        public int ItemId { get; set; }

        public int Amount { get; set; }

        public double Price { get; set; }

        [FieldDef(IgnoreForSave = true)]
        public double TotalItem
        {
            get
            {
                return this.Amount * this.Price;
            }
        }
    }
}