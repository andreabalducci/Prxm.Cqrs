using System;

namespace Sample.Domain.Purchases.Domain.Events
{
    public class BillOfLadingDetailAdded
    {
        public Guid ItemId { get; protected set; }
        public string Sku { get; protected set; }
        public string Description { get; protected set; }
        public decimal Quantity { get; protected set; }

        public BillOfLadingDetailAdded(Guid itemId, string sku, string description, decimal quantity)
        {
            ItemId = itemId;
            Sku = sku;
            Description = description;
            Quantity = quantity;
        }
    }
}