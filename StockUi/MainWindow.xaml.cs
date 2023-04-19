using StockUi.Services;
using StockUi.ViewModel;
using System.Windows;

namespace StockUi
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainViewModel(new StockLiveUpdateService());
            InitializeComponent();
        }
    }
}