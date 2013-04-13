using System.Collections.Generic;

namespace FilConvWpf.Encode
{
    /// <summary>
    /// Implementors handle saving encoded data in particular output formats.
    /// </summary>
    public interface ISaveDelegate
    {
        string FormatNameL10nKey { get; }
        IEnumerable<string> FileNameMasks { get; }
        void SaveAs(string fileName);
    }
}
