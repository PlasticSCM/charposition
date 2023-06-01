namespace charposition.ParserModel;

public class ChildNode
{
    public string? Type { get; set; }
    public string? Name { get; set; }
    public LocationSpan? LocationSpan { get; set; }
    public int[]? Span { get; set; }
    public int[]? HeaderSpan { get; set; }
    public int[]? FooterSpan { get; set; }
    public ChildNode[]? Children { get; set; }
}
