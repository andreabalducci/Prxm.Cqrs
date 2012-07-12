using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Core;
using Sample.Domain.Purchases.Domain.Events;

namespace Sample.Domain.Purchases.Domain
{
    public class BillOfLading : AggregateBase
    {
        public class Supplier
        {
            public string CompanyName { get; protected set; }
            public string Address { get; protected set; }

            public Supplier(string companyName, string address)
            {
                CompanyName = companyName;
                Address = address;
            }
        }

        public class Detail
        {
            public Guid ItemId { get; protected set; }
            public string Sku { get; protected set; }
            public string Description { get; protected set; }
            public decimal Quantity { get; protected set; }

            public Detail(Guid itemId, string sku, string description, decimal quantity)
            {
                ItemId = itemId;
                Sku = sku;
                Description = description;
                Quantity = quantity;
            }
        }

        /// <summary>
        /// Data
        /// </summary>
        public DateTime IssuedAt { get; protected set; }
        public Supplier IssuedBy { get; protected set; }
        public string Number { get; protected set; }
        public DateTime ReceivedAt { get; protected set; }
        public bool Confirmed { get; protected set; }
        public IList<Detail> Details { get; protected set; }

        public BillOfLading()
        {
            this.Details = new List<Detail>();
        }

        public BillOfLading(Guid id, string number, DateTime issuedAt, DateTime receivedAt)
        {
            this.Details = new List<Detail>();
            RaiseEvent(new BillOfLadingCreated(id, issuedAt, number, receivedAt));            
        }

        public void Confirm()
        {
            RaiseEvent(new BillOfLadingConfirmed(this.Id));
        }

        public void SetSupplier(string companyName, string address)
        {
            RaiseEvent(new SupplierChanged(companyName, address));
        }

        public void AddDetail(Guid itemId, string sku, string description, decimal quantity)
        {
            RaiseEvent(new BillOfLadingDetailAdded(itemId, sku, description, quantity));
        }

        private void Apply(BillOfLadingDetailAdded detail)
        {
            this.Details.Add(new Detail(detail.ItemId, detail.Sku, detail.Description, detail.Quantity));
        }

        private void Apply(BillOfLadingCreated @event)
        {
            this.Id = @event.Id;
            this.IssuedAt = @event.IssuedAt;
            this.Number = @event.Number;
            this.ReceivedAt = @event.ReceivedAt;
        }

        private void Apply(BillOfLadingConfirmed evt)
        {
            this.Confirmed = true;
        }

        private void Apply(SupplierChanged evt)
        {
            this.IssuedBy = new Supplier(evt.CompanyName,evt.Address);
        }
    }
}
