using System.Windows.Controls;
using System.Windows.Media;

namespace charposition.Controls;

public class Character : Canvas
{
    public int Line { get; set; }
    public int Column { get; set; }

    public int CharIndex
    {
        set
        {
            this.Index.Text = value.ToString();
        }
    }
    public char CharCode
    {
        set
        {
            string display = DisplayCharacter(value);
            this.Label.Text = display;
            this.Label.Foreground = display != value.ToString() ? Brushes.Gray : Brushes.Black;
        }
    }

    private TextBlock Label { get; }
    private TextBlock Index { get; }

    public Character()
    {
        this.Label = new()
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            VerticalAlignment = System.Windows.VerticalAlignment.Top,
        };
        this.Children.Add(this.Label);
        Canvas.SetTop(this.Label, 2);
        Canvas.SetLeft(this.Label, 8);

        this.Index = new()
        {
            Foreground = Brushes.Gray,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
            VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
            FontSize = 10
        };
        this.Children.Add(this.Index);
        Canvas.SetBottom(this.Index, 2);
        Canvas.SetRight(this.Index, 2);
    }

    private static string DisplayCharacter(char character) =>
        character switch
        {
            '\r' => "\\r",
            '\n' => "\\n",
            '\t' => "\\t",
            _ => character.ToString(),
        };
}
