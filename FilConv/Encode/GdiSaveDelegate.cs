using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;

namespace FilConv.Encode;

class GdiSaveDelegate : ISaveDelegate
{
    private readonly Bitmap _bitmap;

    public GdiSaveDelegate(Bitmap bitmap, string formatNameL10nKey, IEnumerable<string> fileNameSuffixes)
    {
        _bitmap = bitmap;
        FormatNameL10nKey = formatNameL10nKey;
        FileNameSuffixes = fileNameSuffixes.ToList();
    }

    public string FormatNameL10nKey { get; }

    public IEnumerable<string> FileNameSuffixes { get; }

    public void SaveAs(string fileName) => _bitmap.Save(fileName);
}
