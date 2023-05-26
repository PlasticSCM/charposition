using charposition.ParserModel;
using System.Windows;
using System.Windows.Controls;

namespace charposition;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowModel model)
    {
        DataContext = model;
        InitializeComponent();
    }

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is not TreeView tree || this.DataContext is not MainWindowModel model)
        {
            return;
        }

        model.SelectedSpan = tree.SelectedItem switch
        {
            FileNode file => file.LocationSpan,
            ChildNode node => node.LocationSpan,
            _ => null,
        };
    }
}
