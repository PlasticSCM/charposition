using charposition.ParserModel;
using System;
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

    public IEnumerable<Dictionary<int, char>> LineChars
    {
        get => (IEnumerable<Dictionary<int, char>>)GetValue(LineCharsProperty);
        set => SetValue(LineCharsProperty, value);
    }
    public static readonly DependencyProperty LineCharsProperty =
            DependencyProperty.Register("LineChars", typeof(IEnumerable<Dictionary<int, char>>), typeof(CharsCanvas), new PropertyMetadata(null, OnLineCharsChanged));

    static void OnLineCharsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ObservableCollection<Dictionary<int, char>> coll && d is CharsCanvas ctrl)
        {
            coll.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) => ctrl.Draw();
        }
    }

    public LocationSpan? SelectedSpan
    {
        get => (LocationSpan?)GetValue(SelectedSpanProperty);
        set => SetValue(SelectedSpanProperty, value);
    }
    public static readonly DependencyProperty SelectedSpanProperty =
        DependencyProperty.Register("SelectedSpan", typeof(LocationSpan), typeof(CharsCanvas), new PropertyMetadata(null, Redraw));

    private readonly ScrollViewer scrollViewer;

    public CharsCanvas(ScrollViewer scrollViewer)
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

        this.scrollViewer = scrollViewer;
        this.scrollViewer.ScrollChanged += (s, e) => this.Draw();
    }

    protected override Size MeasureOverride(Size constraint) =>
        new(this.MaxLineLength * CharsView.CellWidth, this.LineCount * CharsView.CellHeight);

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
            this.UpdateCharacterSelection(this.HighlightedCharacter);
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
        // Calculate visible columns
        this.visibleLines = (int)Math.Ceiling(this.scrollViewer.ViewportHeight / CharsView.CellHeight) + 2;
        this.visibleColumns = (int)Math.Ceiling(this.scrollViewer.ViewportWidth / CharsView.CellWidth) + 2;

        // Update columns
        this.startRow = (int)Math.Max(0, Math.Floor(this.scrollViewer.VerticalOffset / CharsView.CellHeight) - 1);
        this.startCol = (int)Math.Max(0, Math.Floor(this.scrollViewer.HorizontalOffset / CharsView.CellWidth) - 1);

        this.UpdateRowLines();
        this.UpdateColumnLines();
        this.UpdateCharacters();
    }

    private void UpdateCharacters()
    {
        int charIndex = 0;
        int rowIndex = 0;
        foreach (var lineChars in this.LineChars.Skip(this.startRow).Take(this.visibleLines))
        {
            var endCol = Math.Min(lineChars.Count, this.startCol + this.visibleColumns);
            var col = startCol;
            foreach(var c in lineChars.Skip(this.startCol).Take(endCol))
            {
                if (this.Characters.Count <= charIndex)
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

                var character = this.Characters[charIndex];
                character.CharIndex = c.Key;
                character.CharCode = c.Value;
                character.Line = rowIndex + this.startRow;
                character.Column = col;
                this.UpdateCharacterSelection(character);
                Canvas.SetTop(character, character.Line * CharsView.CellHeight);
                Canvas.SetLeft(character, col * CharsView.CellWidth);

                charIndex++;
                col++;
            }
            rowIndex++;
        }

        // Cleanup unneeded chars
        if (this.Characters.Count > charIndex + 100)
        {
            while (this.Characters.Count > charIndex)
            {
                this.Canvas.Children.Remove(this.Characters[^1]);
                this.Characters.RemoveAt(this.Characters.Count - 1);
            }
        }
    }

    private void UpdateCharacterSelection(Character character)
    {
        bool isSelected = false;
        if (this.SelectedSpan?.Start != null && this.SelectedSpan?.End != null)
        {
            int row = character.Line + 1;
            if (row == this.SelectedSpan.Start[0])
            {
                isSelected = character.Column + 1 >= this.SelectedSpan.Start[1];
            }
            else if (row > this.SelectedSpan.Start[0] && character.Line < this.SelectedSpan.End[0])
            {
                isSelected = true;
            }
            else if (row == this.SelectedSpan.End[0])
            {
                isSelected = character.Column + 1 <= this.SelectedSpan.End[1];
            }
        }

        character.Background = isSelected ? CharsView.SelectionBrush : Brushes.Transparent;
    }

    private void UpdateColumnLines()
    {
        int rowIndex = 0;
        int colIndex = 0;
        foreach (var lineChars in this.LineChars.Skip(this.startRow).Take(this.visibleLines))
        {
            var endCol = Math.Min(lineChars.Count, this.startCol + this.visibleColumns);
            for (int i = this.startCol; i <= endCol; i++)
            {
                if (this.ColumnLines.Count <= colIndex)
                {
                    var line = new Line
                    {
                        Stroke = CharsView.LineBrush,
                        StrokeThickness = 0.5,
                    };
                    this.Canvas.Children.Add(line);
                    this.ColumnLines.Add(line);
                }
                var columnLine = this.ColumnLines[colIndex++];

                columnLine.X1 = columnLine.X2 = (i * CharsView.CellWidth);

                int row = this.startRow + rowIndex;
                columnLine.Y1 = (row * CharsView.CellHeight);
                columnLine.Y2 = ((row + 1) * CharsView.CellHeight);
            }

            rowIndex++;
        }

        // Cleanup unneeded columns
        if (this.ColumnLines.Count > colIndex + 100)
        {
            while (this.ColumnLines.Count > colIndex)
            {
                this.Canvas.Children.Remove(this.ColumnLines[^1]);
                this.ColumnLines.RemoveAt(this.ColumnLines.Count - 1);
            }
        }
    }

    private void UpdateRowLines()
    {
        // Cleanup unneeded lines
        if (this.RowLines.Count > this.visibleLines + 25)
        {
            while (this.RowLines.Count > this.visibleLines)
            {
                this.Canvas.Children.Remove(this.RowLines[^1]);
                this.RowLines.RemoveAt(this.RowLines.Count - 1);
            }
        }

        // Create new lines
        while (this.RowLines.Count < this.visibleLines)
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

        // Adjust line lengths
        int lastLineLength = 0;
        int rowIndex = 0;
        foreach (var lineChars in this.LineChars.Skip(this.startRow).Take(this.visibleLines))
        {
            if (lastLineLength == 0)
            {
                rowIndex++;
                lastLineLength = lineChars.Count;
                continue;
            }

            int length = Math.Max(lastLineLength, lineChars.Count);
            UpdateRowLineLength(rowIndex, this.startRow + rowIndex, length);
            lastLineLength = lineChars.Count;
            rowIndex++;
        }

        UpdateRowLineLength(rowIndex, this.startRow + rowIndex, lastLineLength);
    }

    private void UpdateRowLineLength(int rowIndex, int lineNo, int length)
    {
        if (rowIndex < 1 || rowIndex >= this.RowLines.Count)
        {
            return;
        }

        var line = this.RowLines[rowIndex - 1];
        line.Y1 = lineNo * CharsView.CellHeight;
        line.Y2 = lineNo * CharsView.CellHeight;
        line.X2 = length * CharsView.CellWidth;
    }

    private Canvas Canvas { get; }

    private List<Line> RowLines { get; } = new();
    private List<Line> ColumnLines { get; } = new();

    private List<Character> Characters { get; } = new();
    private Character? HighlightedCharacter { get; set; }

    private Rectangle HighlightLine { get; }
    private Rectangle HighlightColumn { get; }

    private int visibleLines;
    private int visibleColumns;
    private int startRow;
    private int startCol;

    public class CharChangedArgs : EventArgs
    {
        public int? Line { get; set; }
        public int? Column { get; set; }
    }
}
