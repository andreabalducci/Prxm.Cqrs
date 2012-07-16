using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.QueryModel.Inventory
{
    public sealed class LastReceivedGoods
    {
        public DateTime ReceivedAt { get; protected set; }
        public Guid ItemId { get; protected set; }
        public string Sku { get; protected set; }
        public string Description { get; protected set; }
        public decimal ReceivedQuantity { get; protected set; }
        public string Supplier { get; protected set; }

        /// <summary>
        /// private default constructor is needed by nhibernate
        /// </summary>
        private LastReceivedGoods() {    }

        public LastReceivedGoods(DateTime receivedAt, Guid itemId, string sku, string description, decimal receivedQuantity, string supplier)
        {
            ReceivedAt = receivedAt;
            ItemId = itemId;
            Sku = sku;
            Description = description;
            ReceivedQuantity = receivedQuantity;
            Supplier = supplier;
        }
    }
}
