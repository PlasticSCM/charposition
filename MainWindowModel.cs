using System.Collections.ObjectModel;
using System.Windows;

namespace charposition;

internal class MainWindowModel : DependencyObject
{
    public int MaximumLineLength
    {
        get => (int)GetValue(MaximumLineLengthProperty);
        set => SetValue(MaximumLineLengthProperty, value);
    }
    public static readonly DependencyProperty MaximumLineLengthProperty =
        DependencyProperty.Register("MaximumLineLength", typeof(int), typeof(MainWindowModel), new PropertyMetadata(0));

    public int LineCount
    {
        get => (int)GetValue(LineCountProperty);
        set => SetValue(LineCountProperty, value);
    }
    public static readonly DependencyProperty LineCountProperty =
        DependencyProperty.Register("LineCount", typeof(int), typeof(MainWindowModel), new PropertyMetadata(0));

    public ObservableCollection<char[]> LineChars { get; } = new();
}
