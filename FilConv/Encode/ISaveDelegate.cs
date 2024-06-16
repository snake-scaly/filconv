using System.Collections.Generic;

namespace FilConv.Encode;

/// <summary>
/// Implementors handle saving encoded data in particular output formats.
/// </summary>
public interface ISaveDelegate
{
    string FormatNameL10nKey { get; }
    IEnumerable<string> FileNameSuffixes { get; }
    void SaveAs(string fileName);
}
