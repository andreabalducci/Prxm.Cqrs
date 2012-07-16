using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sample.Commands.Inventory;
using Sample.QueryModel.Inventory;
using System.Collections.ObjectModel;
using Sample.QueryModel.NHibernate;
using NHibernate.Linq;
using NHibernate;

namespace Sample.Client.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new PrimitiveAndUglyViewModel();
        }

        private void CreateNewItem(object sender, RoutedEventArgs e)
        {
            var id = Guid.NewGuid();
            var command = new CreateInventoryItemCommand(id)
            {
                ItemId = id,
                Sku = txtNewSkuDescription.Text,
                Description = txtNewSkuDescription.Text,
            };


            Infrastructure.Instance.SendCommand(command);
        }
    }

    public class PrimitiveAndUglyViewModel
    {

        public ObservableCollection<InventoryItemTotalQuantity> InventoryTotalItemView { get; set; }

        public PrimitiveAndUglyViewModel()
        {
            InventoryTotalItemView = new ObservableCollection<InventoryItemTotalQuantity>();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                foreach (var item in session.Query<InventoryItemTotalQuantity>().ToList())
                {
                    InventoryTotalItemView.Add(item);
                }
            }
        }
    }
}
