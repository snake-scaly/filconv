using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using ImageLib;

namespace FilConv.Encode;

public class RawSaveDelegate : ISaveDelegate
{
    private readonly Bitmap _bitmap;
    private readonly INativeImageFormat _format;
    private readonly EncodingOptions _options;

    public string FormatNameL10nKey => "FileFormatNameRaw";

    public IEnumerable<string> FileNameSuffixes => [string.Empty];

    public RawSaveDelegate(Bitmap bitmap, INativeImageFormat format, EncodingOptions options)
    {
        _bitmap = bitmap;
        _format = format;
        _options = options;
    }

    public void SaveAs(string fileName)
    {
        var nativeImage = _format.ToNative(new BitmapPixels(_bitmap), _options);
        File.WriteAllBytes(fileName, nativeImage.Data);
    }
}
