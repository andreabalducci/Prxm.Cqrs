using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.QueryModel.Inventory
{
    /// <summary>
    /// TODO: Avoid calling with the same name of domain entity, great confusion it will bring. (Yoda mode on!)
    /// </summary>
    public class InventoryItem
    {
        public class Availability
        {
            public string Storage { get; protected set; }
            public decimal Quantity { get; protected set; }

            /// <summary>
            /// Default constructor even private is needed by nhibernate
            /// </summary>
            private Availability() { }

            public Availability(string storage, decimal quantity)
            {
                Storage = storage;
                Quantity = quantity;
            }
        }

        public Guid Id { get; protected set; }
        public string Sku { get; protected set; }
        public string Description { get; protected set; }
        public IList<Availability> StorageAvailability { get; protected set; }

        /// <summary>
        /// Default constructor even private is needed by nhibernate
        /// </summary>
        private InventoryItem() { }

        public InventoryItem(Guid id, string sku, string description)
        {
            Id = id;
            Sku = sku;
            Description = description;
            this.StorageAvailability = new List<Availability>();
        }
    }
}
