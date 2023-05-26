namespace charposition.ParserModel;

public class FileNode
{
    public string? Type { get; set; }
    public string? Name { get; set; }
    public LocationSpan? LocationSpan { get; set; }
    public int[]? Span { get; set; }
    public int[]? FooterSpan { get; set; }
    public bool ParsingErrorsDetected { get; set; }
    public int[]? ParsingErrors { get; set; }
    public ChildNode[]? Children { get; set; }
}
