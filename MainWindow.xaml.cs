using charposition.Services;
using System;
using System.Windows;

namespace charposition;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowModel model;
    private readonly ILineSplitter lineSplitter;

    public MainWindow(ILineSplitter lineSplitter)
    {
        DataContext = model = new MainWindowModel();
        InitializeComponent();
        this.lineSplitter = lineSplitter;
    }

    public void SetText(string text)
    {
        model.MaximumLineLength = 0;
        model.LineCount = 0;
        model.LineChars.Clear();

        foreach (char[] line in lineSplitter.SplitLines(text))
        {
            model.LineCount++;
            model.MaximumLineLength = Math.Max(model.MaximumLineLength, line.Length);
            model.LineChars.Add(line);
        }
    }
}
