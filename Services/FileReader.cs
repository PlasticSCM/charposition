using System.IO;

namespace charposition.Services;

internal class FileReader : IFileReader
{
    public string ReadAllText(string path) => File.ReadAllText(path);
}
