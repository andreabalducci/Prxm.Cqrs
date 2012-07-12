namespace Sample.Domain.Purchases.Domain.Events
{
    public class BillOfLadingDetailAdded
    {
        public string Sku { get; protected set; }
        public string Description { get; protected set; }
        public decimal Quantity { get; protected set; }

        public BillOfLadingDetailAdded(string sku, string description, decimal quantity)
        {
            Sku = sku;
            Description = description;
            Quantity = quantity;
        }
    }
}