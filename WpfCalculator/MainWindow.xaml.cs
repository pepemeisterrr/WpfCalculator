using System.Windows;
using WpfCalculator.ViewModels;

namespace WpfCalculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}