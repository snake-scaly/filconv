using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace FilConv.Encode;

class GdiSaveDelegate : SaveDelegateAbstr
{
    private Bitmap _bitmap;
    private string _name;
    private IEnumerable<string> _masks;

    public GdiSaveDelegate(Bitmap bitmap, string formatNameL10nKey, IEnumerable<string> fileNameMasks)
    {
        _bitmap = bitmap;
        _name = formatNameL10nKey;
        _masks = new List<string>(fileNameMasks).AsReadOnly();
    }

    public override string FormatNameL10nKey => _name;

    public override IEnumerable<string> FileNameMasks => _masks;

    public override void SaveAs(string fileName) => _bitmap.Save(fileName);
}
