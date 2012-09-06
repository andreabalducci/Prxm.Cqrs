using System.Windows;
using LogVisualizer.ViewModels;

namespace LogVisualizer.Views
{
    /// <summary>
    /// Interaction logic for RawLoggerView.xaml
    /// </summary>
    public partial class RawLoggerView : Window
    {
        public RawLoggerView(RawLoggerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
