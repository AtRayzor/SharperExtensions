using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Analyzers.Tests;

internal static class SourceTextFactory
{
    public static SourceText CreateSourceText(string source)
    {
        var stream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read);
        return SourceText.From(stream, Encoding.UTF8);
    }
}