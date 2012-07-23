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
using System.Windows.Shapes;
using Sample.DebugUi.ViewModels;

namespace Sample.DebugUi.Views
{
    /// <summary>
    /// Interaction logic for RawLoggerView.xaml
    /// </summary>
    public partial class CommandView : Window
    {
        public CommandView(RawLoggerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
