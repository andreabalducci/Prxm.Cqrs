using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Commands.Purchases
{
    public class RegisterBillOfLadingCommandBuilder
    {
        private RegisterBillOfLadingCommand _command;
        private IList<RegisterBillOfLadingCommand.Row> _rows;
        public RegisterBillOfLadingCommandBuilder(Guid commandId)
        {
            _command = new RegisterBillOfLadingCommand(commandId);
            _rows = new List<RegisterBillOfLadingCommand.Row>();
        }

        public RegisterBillOfLadingCommandBuilder From(string supplier, string address)
        {
            CheckValidState();
            _command.SupplierCompanyName = supplier;
            _command.SupplierAddress = address;
            return this;
        }

        public RegisterBillOfLadingCommandBuilder IssuedAt(DateTime issued)
        {
            CheckValidState();
            _command.IssueDate = issued;
            return this;
        }

        public RegisterBillOfLadingCommandBuilder Numbered(string number)
        {
            CheckValidState();
            _command.Number = number;
            return this;
        }

        public RegisterBillOfLadingCommandBuilder AddRow(Guid itemId, string sku, string description, decimal quantity)
        {
            CheckValidState();
            _rows.Add(new RegisterBillOfLadingCommand.Row(itemId, sku, description, quantity));
            return this;
        }

        public RegisterBillOfLadingCommand Build()
        {
            CheckValidState();

            _command.Rows = _rows.ToArray();
            var cmd = _command;
            _command = null;
            return cmd;
        }

        private void CheckValidState()
        {
            if (_command == null)
                throw new Exception("command already build");
        }
    }

    public class RegisterBillOfLadingCommand : ICommand
    {
        public class Row
        {
            public Guid ItemId { get; protected set; }
            public string Sku { get; protected set; }
            public string Description { get; protected set; }
            public decimal Quantity { get; protected set; }

            protected Row()
            {
            }

            public Row(Guid itemid, string sku, string description, decimal quantity)
            {
                ItemId = itemid;
                Sku = sku;
                Description = description;
                Quantity = quantity;
            }
        }

        public Guid Id { get; protected set; }
        public string Number { get; internal set; }
        public string SupplierCompanyName { get; internal set; }
        public DateTime IssueDate { get; internal set; }
        public string SupplierAddress{ get; internal set; }

        public Row[] Rows { get; internal set; }

        public RegisterBillOfLadingCommand()
        {
        }

        public RegisterBillOfLadingCommand(Guid id)
        {
            Id = id;
        }
    }
}
