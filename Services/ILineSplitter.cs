using System.Collections.Generic;

namespace charposition.Services;

public interface ILineSplitter
{
    IEnumerable<char[]> SplitLines(string text);
}
