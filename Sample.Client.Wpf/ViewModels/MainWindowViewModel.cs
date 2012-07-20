using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Client.Wpf.KissMvvm;
using System.Collections.ObjectModel;
using Sample.QueryModel.Inventory;
using NHibernate;
using Sample.QueryModel.NHibernate;
using NHibernate.Linq;
using Sample.Commands.Inventory;

namespace Sample.Client.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {

        public ObservableCollection<InventoryItemTotalQuantity> InventoryTotalItemView { get; set; }

        public MainWindowViewModel()
        {
            InventoryTotalItemView = new ObservableCollection<InventoryItemTotalQuantity>();
            PropertyLink.OnObject(this)
                .Link(vm => vm.NewInventoryItemDescription, "CanExecuteCreateNewInventoryItem")
                .Link(vm => vm.NewInventoryItemSku, "CanExecuteCreateNewInventoryItem")
                .Link(vm => vm.SelectedInventoryTotalItemView, "CanExecuteAddQuantity")
                .Link(vm => vm.QuantityToAddToSelectedItem, "CanExecuteAddQuantity");
        }

        public InventoryItemTotalQuantity SelectedInventoryTotalItemView
        {
            get { return _SelectedInventoryTotalItemView; }
            set { this.Set(p => p.SelectedInventoryTotalItemView, value, ref _SelectedInventoryTotalItemView); }
        }

        private InventoryItemTotalQuantity _SelectedInventoryTotalItemView;

        public Decimal QuantityToAddToSelectedItem
        {
            get { return _QuantityToAddToSelectedItem; }
            set { this.Set(p => p.QuantityToAddToSelectedItem, value, ref _QuantityToAddToSelectedItem); }
        }

        private Decimal _QuantityToAddToSelectedItem;


        #region Bindable properties

        public String NewInventoryItemSku
        {
            get { return _NewInventoryItemSku; }
            set { this.Set(p => p.NewInventoryItemSku, value, ref _NewInventoryItemSku); }
        }
        private String _NewInventoryItemSku;

        public String NewInventoryItemDescription
        {
            get { return _NewInventoryItemDescription; }
            set { this.Set(p => p.NewInventoryItemDescription, value, ref _NewInventoryItemDescription); }
        }

        private String _NewInventoryItemDescription;
        #endregion

        #region Commands

        public Boolean CanExecuteCreateNewInventoryItem(Object state)
        {

            return !String.IsNullOrEmpty(NewInventoryItemDescription) && !String.IsNullOrEmpty(NewInventoryItemSku);
        }

        public void ExecuteCreateNewInventoryItem(Object state)
        {

            var id = Guid.NewGuid();
            var command = new CreateInventoryItemCommand(id)
            {
                ItemId = id,
                Sku = NewInventoryItemSku,
                Description = NewInventoryItemDescription,
            };

            Infrastructure.Instance.SendCommand(command);
        }

        public void ExecuteLoad(Object state)
        {
            InventoryTotalItemView.Clear();
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in session.Query<InventoryItemTotalQuantity>().ToList())
                    {
                        InventoryTotalItemView.Add(item);
                    }
                }
            }
            catch (Exception)
            {

                //todo: log the error
            }
        }

        public Boolean CanExecuteAddQuantity(object param)
        {
            return SelectedInventoryTotalItemView != null &&
                QuantityToAddToSelectedItem > 0;

        }
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExecuteAddQuantity(object param)
        {

            StockIncomingItemCommand command = new StockIncomingItemCommand(
                Guid.NewGuid(),
                SelectedInventoryTotalItemView.Id,
                SelectedInventoryTotalItemView.Sku,
                "",
                QuantityToAddToSelectedItem,
                "ASTORAGE");

            Infrastructure.Instance.SendCommand(command);
        }
        #endregion
    }
}
