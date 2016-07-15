using System;
using System.Collections.ObjectModel;
using System.Linq;
using Rednet.DataAccess;

namespace TestApp.Objects
{
    public class Purchase : DatabaseObject<Purchase>
    {
        [FieldDef(AutomaticValue = AutomaticValue.AutoIncrement, IsPrimaryKey = true)]
        public int PurchaseId { get; set; }

        public DateTime PurchaseDate { get; set; }

        [FieldDef(IgnoreForSave = true)]
        public double TotalPurchase
        {
            get { return this.OrderItems.Sum(i => i.TotalItem); }
        }

        [JoinField(SourceColumnNames = new [] { "PurchaseId" }, TargetColumnNames = new [] { "PurchaseId" }, JoinRelation = JoinRelation.OneToMany, JoinType = JoinType.LeftJoin)]
        public ObservableCollection<PurchaseItem> OrderItems { get; set; } 
    }
}