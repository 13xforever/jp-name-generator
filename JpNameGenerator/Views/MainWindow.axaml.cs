using Avalonia.Controls;
using Avalonia.Interactivity;
using JpNameGenerator.ViewModels;

namespace JpNameGenerator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not MainWindowViewModel vm)
            return;

        vm.GenerateNameInBgAsync();
    }
}