using System.Windows.Controls;
using System.Windows.Media;

namespace charposition.Controls;

public class Character : Canvas
{
    public Character(int totalChars, char character)
    {
        string display = DisplayCharacter(character);
        TextBlock label = new()
        {
            Text = display,
            Foreground = display != character.ToString() ? Brushes.Gray : Brushes.Black,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            VerticalAlignment = System.Windows.VerticalAlignment.Top,
        };
        this.Children.Add(label);
        Canvas.SetTop(label, 2);
        Canvas.SetLeft(label, 8);

        TextBlock index = new()
        {
            Text = totalChars.ToString(),
            Foreground = Brushes.Gray,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
            VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
            FontSize = 10
        };
        this.Children.Add(index);
        Canvas.SetBottom(index, 2);
        Canvas.SetRight(index, 2);
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
