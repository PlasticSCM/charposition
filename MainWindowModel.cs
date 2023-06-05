using charposition.ParserModel;
using charposition.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace charposition;

public class MainWindowModel : DependencyObject
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

    public FileNode? SemanticFile
    {
        get => (FileNode?)GetValue(SemanticFileProperty);
        set => SetValue(SemanticFileProperty, value);
    }
    public static readonly DependencyProperty SemanticFileProperty =
        DependencyProperty.Register("SemanticFile", typeof(FileNode), typeof(MainWindowModel), new PropertyMetadata(null));

    public string? ErrorMessage
    {
        get => (string?)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }
    public static readonly DependencyProperty ErrorMessageProperty =
        DependencyProperty.Register("ErrorMessage", typeof(string), typeof(MainWindowModel), new PropertyMetadata(null));

    public LocationSpan? SelectedSpan
    {
        get => (LocationSpan?)GetValue(SelectedSpanProperty);
        set => SetValue(SelectedSpanProperty, value);
    }
    public static readonly DependencyProperty SelectedSpanProperty =
        DependencyProperty.Register("SelectedSpan", typeof(LocationSpan), typeof(MainWindowModel), new PropertyMetadata(null));

    public ObservableCollection<FileNode> Semantics { get; } = new();

    public ObservableCollection<Dictionary<int, char>> LineChars
    {
        get => (ObservableCollection<Dictionary<int, char>>)GetValue(LineCharsProperty);
        set => SetValue(LineCharsProperty, value);
    }
    public static readonly DependencyProperty LineCharsProperty =
        DependencyProperty.Register("LineChars", typeof(ObservableCollection<Dictionary<int, char>>), typeof(MainWindowModel),
            new PropertyMetadata(new ObservableCollection<Dictionary<int, char>>()));

    private readonly ILineSplitter lineSplitter;

    public MainWindowModel(ILineSplitter lineSplitter)
    {
        this.lineSplitter = lineSplitter;
    }

    public void LoadData(string text, string semantics)
    {
        this.Clear();
        this.LoadText(text);
        this.LoadSemantics(semantics);
    }

    private void LoadSemantics(string semantics)
    {
        if (string.IsNullOrEmpty(semantics))
        {
            return;
        }

        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            this.SemanticFile = deserializer.Deserialize<FileNode>(new StringReader(semantics));
            this.Semantics.Add(this.SemanticFile);
        }
        catch (Exception ex)
        {
            this.ErrorMessage = ex.Message;
            if (ex.InnerException != null)
            {
                this.ErrorMessage += $": {ex.InnerException.Message}";
            }
        }
    }

    private void LoadText(string text)
    {
        int lineCount = 0;
        int maxLineLen = 0;
        int charIndex = 0;
        List<Dictionary<int, char>> lines = new();
        foreach (char[] lineChars in lineSplitter.SplitLines(text))
        {
            lineCount++;
            maxLineLen = Math.Max(maxLineLen, lineChars.Length);

            Dictionary<int, char> line = new();
            foreach(char c in lineChars)
            {
                line[charIndex++] = c;
            }
            lines.Add(line);
        }

        this.LineCount = lineCount;
        this.MaximumLineLength = maxLineLen;
        this.LineChars = new ObservableCollection<Dictionary<int, char>>(lines);
    }

    private void Clear()
    {
        this.ErrorMessage = null;
        this.SemanticFile = null;
        this.Semantics.Clear();
        this.MaximumLineLength = 0;
        this.LineCount = 0;
        this.LineChars.Clear();
    }
}
