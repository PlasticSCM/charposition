using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace charposition.Controls;

public class CharsCanvas : UserControl
{
    public event EventHandler<CharChangedArgs>? HoveredCharChanged;

    public int LineCount
    {
        get => (int)GetValue(LineCountProperty);
        set => SetValue(LineCountProperty, value);
    }
    public static readonly DependencyProperty LineCountProperty =
        DependencyProperty.Register("LineCount", typeof(int), typeof(CharsCanvas), new PropertyMetadata(1, Redraw));

    public int MaxLineLength
    {
        get => (int)GetValue(MaxLineLengthProperty);
        set => SetValue(MaxLineLengthProperty, value);
    }
    public static readonly DependencyProperty MaxLineLengthProperty =
        DependencyProperty.Register("MaxLineLength", typeof(int), typeof(CharsCanvas), new PropertyMetadata(1, Redraw));

    public IEnumerable LineChars
    {
        get => (IEnumerable)GetValue(LineCharsProperty);
        set => SetValue(LineCharsProperty, value);
    }
    public static readonly DependencyProperty LineCharsProperty =
            DependencyProperty.Register("LineChars", typeof(IEnumerable), typeof(CharsCanvas), new PropertyMetadata(null, OnLineCharsChanged));

    static void OnLineCharsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ObservableCollection<char[]> coll && d is CharsCanvas ctrl)
        {
            coll.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) => ctrl.Draw();
        }
    }

    public CharsCanvas()
    {
        this.Background = Brushes.Transparent;

        this.Canvas = new Canvas();
        this.Content = this.Canvas;

        Canvas.SetBinding(WidthProperty,
                new Binding { Path = new PropertyPath(ActualWidthProperty), Source = this });
        Canvas.SetBinding(HeightProperty,
            new Binding { Path = new PropertyPath(ActualHeightProperty), Source = this });

        this.SizeChanged += (s, e) => this.Draw();

        this.HighlightColumn = new()
        {
            Width = CharsView.CellWidth,
            Fill = CharsView.HighlightBrush
        };
        this.HighlightLine = new()
        {
            Height = CharsView.CellHeight,
            Fill = CharsView.HighlightBrush
        };

        this.HighlightLine.SetBinding(WidthProperty,
                new Binding { Path = new PropertyPath(ActualWidthProperty), Source = this });
        this.HighlightColumn.SetBinding(HeightProperty,
            new Binding { Path = new PropertyPath(ActualHeightProperty), Source = this });
    }

    protected override Size MeasureOverride(Size constraint) =>
        new(this.ColumnLines.Max(l => l.X1), this.RowLines.Max(r => r.Y1));

    protected override void OnMouseMove(MouseEventArgs e)
    {
        var mouseOverChar = this.Characters.Find(c => c.IsMouseOver);
        this.HighlightCharacter(mouseOverChar);
    }

    internal void HighlightCharacter(Character? character)
    {
        if (this.HighlightedCharacter == character)
        {
            return;
        }

        this.HoveredCharChanged?.Invoke(this, new CharChangedArgs
        {
            Column = character?.Column,
            Line = character?.Line
        });

        if (this.HighlightedCharacter != null)
        {
            this.HighlightedCharacter.Background = Brushes.Transparent;
        }

        this.HighlightedCharacter = character;
        if (character == null)
        {
            this.Canvas.Children.Remove(this.HighlightColumn);
            this.Canvas.Children.Remove(this.HighlightLine);
            return;
        }

        character.Background = Brushes.Lime;
        Canvas.SetLeft(this.HighlightColumn, character.Column * CharsView.CellWidth);
        Canvas.SetTop(this.HighlightLine, character.Line * CharsView.CellHeight);
        if (!this.Canvas.Children.Contains(this.HighlightColumn))
        {
            this.Canvas.Children.Insert(0, this.HighlightColumn);
        }
        if (!this.Canvas.Children.Contains(this.HighlightLine))
        {
            this.Canvas.Children.Insert(0, this.HighlightLine);
        }
    }

    private static void Redraw(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((CharsCanvas)d).Draw();

    private void Draw()
    {
        this.UpdateRowLines();
        this.UpdateColumnLines();
        this.UpdateCharacters();
    }

    private void UpdateCharacters()
    {
        int totalChars = 0;
        int lineNo = 0;
        foreach (char[] chars in this.LineChars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (this.Characters.Count <= totalChars)
                {
                    var @char = new Character()
                    {
                        Width = CharsView.CellWidth,
                        Height = CharsView.CellHeight,
                        Background = Brushes.Transparent,
                    };
                    this.Canvas.Children.Add(@char);
                    this.Characters.Add(@char);
                }

                var character = this.Characters[totalChars];
                character.CharIndex = totalChars;
                character.CharCode = chars[i];
                character.Line = lineNo;
                character.Column = i;
                Canvas.SetTop(character, lineNo * CharsView.CellHeight);
                Canvas.SetLeft(character, i * CharsView.CellWidth);

                totalChars++;
            }
            lineNo++;
        }

        while (this.Characters.Count > totalChars)
        {
            this.Canvas.Children.Remove(this.Characters[^1]);
            this.Characters.RemoveAt(this.Characters.Count - 1);
        }
    }

    private void UpdateColumnLines()
    {
        int row = 0;
        int index = 0;
        foreach (char[] lineChars in this.LineChars)
        {
            for (int i = 1; i <= lineChars.Length; i++)
            {
                if (this.ColumnLines.Count <= index)
                {
                    var line = new Line
                    {
                        Stroke = CharsView.LineBrush,
                        StrokeThickness = 0.5,
                    };
                    this.Canvas.Children.Add(line);
                    this.ColumnLines.Add(line);
                }
                var columnLine = this.ColumnLines[index++];

                columnLine.X1 = columnLine.X2 = (i * CharsView.CellWidth);
                columnLine.Y1 = (row * CharsView.CellHeight);
                columnLine.Y2 = ((row + 1) * CharsView.CellHeight);
            }

            row++;
        }

        while (this.ColumnLines.Count > index)
        {
            this.Canvas.Children.Remove(this.ColumnLines[^1]);
            this.ColumnLines.RemoveAt(this.ColumnLines.Count - 1);
        }
    }

    private void UpdateRowLines()
    {
        while (this.RowLines.Count > this.LineCount)
        {
            this.Canvas.Children.Remove(this.RowLines[^1]);
            this.RowLines.RemoveAt(this.RowLines.Count - 1);
        }
        while (this.RowLines.Count < this.LineCount)
        {
            var line = new Line
            {
                Stroke = CharsView.LineBrush,
                StrokeThickness = 0.5,
                X1 = 0
            };
            this.Canvas.Children.Add(line);
            this.RowLines.Add(line);
        }
        int row = 0;
        int lastLineLength = 0;
        foreach (char[] lineChars in this.LineChars)
        {
            if (row == 0)
            {
                row++;
                lastLineLength = lineChars.Length;
                continue;
            }
            int length = Math.Max(lastLineLength, lineChars.Length);
            UpdateRowLineLength(row, length);
            lastLineLength = lineChars.Length;
            row++;
        }
        UpdateRowLineLength(row, lastLineLength);
    }

    private void UpdateRowLineLength(int row, int length)
    {
        if (row < 1)
        {
            return;
        }

        var line = this.RowLines[row - 1];
        line.Y1 = row * CharsView.CellHeight;
        line.Y2 = row * CharsView.CellHeight;
        line.X2 = length * CharsView.CellWidth;
    }

    private Canvas Canvas { get; }

    private List<Line> RowLines { get; } = new();
    private List<Line> ColumnLines { get; } = new();

    private List<Character> Characters { get; } = new();
    private Character? HighlightedCharacter { get; set; }

    private Rectangle HighlightLine { get; }
    private Rectangle HighlightColumn { get; }

    public class CharChangedArgs : EventArgs
    {
        public int? Line { get; set; }
        public int? Column { get; set; }
    }
}
