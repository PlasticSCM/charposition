using charposition.Converters;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace charposition.Controls;

public class CharsView : UserControl
{
    internal const int CellWidth = 24;
    internal const int CellHeight = 32;

    internal static readonly Brush LineBrush = Brushes.Gray;
    internal static readonly Brush LabelBrush = Brushes.Gray;
    internal static readonly Brush HighlightBrush = new SolidColorBrush(Color.FromRgb(230, 255, 236));

    public int LineCount
    {
        get => (int)GetValue(LineCountProperty);
        set => SetValue(LineCountProperty, value);
    }
    public static readonly DependencyProperty LineCountProperty =
        DependencyProperty.Register("LineCount", typeof(int), typeof(CharsView), new PropertyMetadata(1, Redraw));

    public int MaxLineLength
    {
        get => (int)GetValue(MaxLineLengthProperty);
        set => SetValue(MaxLineLengthProperty, value);
    }
    public static readonly DependencyProperty MaxLineLengthProperty =
        DependencyProperty.Register("MaxLineLength", typeof(int), typeof(CharsView), new PropertyMetadata(1, Redraw));

    public IEnumerable LineChars
    {
        get => (IEnumerable)GetValue(LineCharsProperty);
        set => SetValue(LineCharsProperty, value);
    }
    public static readonly DependencyProperty LineCharsProperty =
            DependencyProperty.Register("LineChars", typeof(IEnumerable), typeof(CharsView), new PropertyMetadata(null, OnLineCharsChanged));

    static void OnLineCharsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ObservableCollection<char[]> coll && d is CharsView ctrl)
        {
            coll.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) => ctrl.Draw();
        }
    }

    public CharsView()
    {
        this.Background = Brushes.Transparent;

        this.Canvas = new Canvas();
        this.Content = this.Canvas;

        Canvas.SetBinding(WidthProperty,
                new Binding { Path = new PropertyPath(ActualWidthProperty), Source = this });
        Canvas.SetBinding(HeightProperty,
            new Binding { Path = new PropertyPath(ActualHeightProperty), Source = this });

        this.SizeChanged += (s, e) => this.Draw();

        this.HorizontalLine = new Line
        {
            Stroke = LineBrush,
            StrokeThickness = 1,
            X1 = CellWidth,
            Y1 = CellWidth,
            Y2 = CellWidth
        };
        this.Canvas.Children.Add(this.HorizontalLine);

        this.VerticalLine = new Line
        {
            Stroke = LineBrush,
            StrokeThickness = 1,
            X1 = CellWidth,
            X2 = CellWidth,
            Y1 = CellWidth
        };
        this.Canvas.Children.Add(this.VerticalLine);

        this.ScrollViewer = new()
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
            VerticalScrollBarVisibility = ScrollBarVisibility.Visible
        };
        this.ScrollViewer.ScrollChanged += (s, e) => this.Draw();
        Canvas.SetTop(this.ScrollViewer, CellWidth + 1);
        Canvas.SetLeft(this.ScrollViewer, CellWidth + 1);
        ScrollViewer.SetBinding(WidthProperty,
            new Binding
            {
                Path = new PropertyPath(ActualWidthProperty),
                Source = this,
                Converter = new SizeAdjustmentConverter { Adjustment = -1 * CellWidth }
            });
        ScrollViewer.SetBinding(HeightProperty,
            new Binding
            {
                Path = new PropertyPath(ActualHeightProperty),
                Source = this,
                Converter = new SizeAdjustmentConverter { Adjustment = -1 * CellWidth }
            });
        this.Canvas.Children.Add(this.ScrollViewer);

        this.CharsCanvas = new();
        this.CharsCanvas.HoveredCharChanged += (s, e) => HighlightCharacter(e.Line, e.Column);
        this.ScrollViewer.Content = this.CharsCanvas;

        CharsCanvas.SetBinding(CharsCanvas.LineCharsProperty,
            new Binding { Path = new PropertyPath(LineCharsProperty), Source = this });
        CharsCanvas.SetBinding(CharsCanvas.MaxLineLengthProperty,
            new Binding { Path = new PropertyPath(MaxLineLengthProperty), Source = this });
        CharsCanvas.SetBinding(CharsCanvas.LineCountProperty,
            new Binding { Path = new PropertyPath(LineCountProperty), Source = this });

        this.HighlightColumn = new()
        {
            Width = CellWidth,
            Height = CellWidth,
            Fill = HighlightBrush,
            Visibility = Visibility.Collapsed
        };
        this.Canvas.Children.Add(this.HighlightColumn);
        this.HighlightLine = new()
        {
            Width = CellWidth,
            Height = CellHeight,
            Fill = HighlightBrush,
            Visibility = Visibility.Collapsed
        };
        this.Canvas.Children.Add(this.HighlightLine);
    }

    private void HighlightCharacter(int? line, int? column)
    {
        if (line.HasValue)
        {
            this.HighlightLine.Visibility = Visibility.Visible;
            Canvas.SetTop(this.HighlightLine, CellWidth + (line.Value * CellHeight) - this.ScrollViewer.VerticalOffset);
        }
        else
        {
            this.HighlightLine.Visibility = Visibility.Collapsed;
        }

        if (column.HasValue)
        {
            this.HighlightColumn.Visibility = Visibility.Visible;
            Canvas.SetLeft(this.HighlightColumn, ((column.Value + 1) * CellWidth) - this.ScrollViewer.HorizontalOffset);
        }
        else
        {
            this.HighlightColumn.Visibility = Visibility.Collapsed;
        }
    }

    private static void Redraw(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((CharsView)d).Draw();

    private void Draw()
    {
        this.UpdateBorder();
        this.UpdateLineNumbers();
        this.UpdateColumnNumbers();
    }

    private void UpdateColumnNumbers()
    {
        while (this.ColumnLabels.Count > this.MaxLineLength)
        {
            this.Canvas.Children.Remove(this.ColumnLabels[^1]);
            this.ColumnLabels.RemoveAt(this.ColumnLabels.Count - 1);
        }
        while (this.ColumnLabels.Count < this.MaxLineLength)
        {
            var label = new TextBlock
            {
                Text = (this.ColumnLabels.Count + 1).ToString(),
                Foreground = LabelBrush,
                FontSize = 12,
                Width = CellWidth,
                Height = CellHeight,
                TextAlignment = TextAlignment.Center
            };
            this.Canvas.Children.Add(label);
            this.ColumnLabels.Add(label);
            Canvas.SetTop(label, 0);
        }

        for (int i = 0; i < this.ColumnLabels.Count; i++)
        {
            var label = this.ColumnLabels[i];
            double left = ((i + 1) * CellWidth) - this.ScrollViewer.HorizontalOffset;
            Canvas.SetLeft(label, left);
            label.Visibility = left >= CellWidth / 2 ? Visibility.Visible : Visibility.Hidden;
        }
    }

    private void UpdateLineNumbers()
    {
        while (this.LineNoLabels.Count > this.LineCount)
        {
            this.Canvas.Children.Remove(this.LineNoLabels[^1]);
            this.LineNoLabels.RemoveAt(this.LineNoLabels.Count - 1);
        }
        while (this.LineNoLabels.Count < this.LineCount)
        {
            var label = new TextBlock
            {
                Text = (this.LineNoLabels.Count + 1).ToString(),
                Foreground = LabelBrush,
                FontSize = 12,
                Width = CellWidth,
                Height = CellHeight,
                TextAlignment = TextAlignment.Center
            };
            this.Canvas.Children.Add(label);
            this.LineNoLabels.Add(label);
            Canvas.SetLeft(label, 0);
        }

        for (int i = 0; i < this.LineNoLabels.Count; i++)
        {
            var label = this.LineNoLabels[i];
            double top = ((i + 1) * CellHeight) - this.ScrollViewer.VerticalOffset;
            Canvas.SetTop(label, top);
            label.Visibility = top >= CellWidth / 2 ? Visibility.Visible : Visibility.Hidden;
        }
    }

    private void UpdateBorder()
    {
        this.HorizontalLine.X2 = this.ActualWidth;
        this.VerticalLine.Y2 = this.ActualHeight;
    }

    private Canvas Canvas { get; }
    private Line HorizontalLine { get; }
    private Line VerticalLine { get; }

    private List<TextBlock> LineNoLabels { get; } = new();
    private List<TextBlock> ColumnLabels { get; } = new();

    private ScrollViewer ScrollViewer { get; }
    private CharsCanvas CharsCanvas { get; }

    private Rectangle HighlightLine { get; }
    private Rectangle HighlightColumn { get; }
}
