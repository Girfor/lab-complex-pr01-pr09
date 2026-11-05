using System.Windows;
using AIS.UI.ViewModels;

namespace AIS.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var vm = new ReagentListViewModel();
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadAsync();
    }
}
