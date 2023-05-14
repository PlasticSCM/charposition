using System.Collections.Generic;

namespace charposition.Services;

internal class LineSplitter : ILineSplitter
{
    public IEnumerable<char[]> SplitLines(string text)
    {
        int index;
        while ((index = text.IndexOf('\n')) > -1)
        {
            yield return text[..(index + 1)].ToCharArray();
            text = text[(index + 1)..];
        }
        yield return text.ToCharArray();
    }
}
