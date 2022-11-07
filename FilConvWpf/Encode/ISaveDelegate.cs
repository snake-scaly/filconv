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
        string DeriveOutputFileName(string inputFileName);
        void SaveAs(string fileName);
    }
}
