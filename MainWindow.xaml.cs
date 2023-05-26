using charposition.Services;
using System;
using System.Windows;

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
}
