using System.Windows;
using LogVisualizer.ViewModels;

namespace LogVisualizer.Views
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
