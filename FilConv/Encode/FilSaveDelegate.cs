using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using FilLib;
using ImageLib;

namespace FilConv.Encode;

class FilSaveDelegate : ISaveDelegate
{
    private readonly Bitmap _bitmap;
    private readonly INativeImageFormat _format;
    private readonly EncodingOptions _options;

    public FilSaveDelegate(Bitmap bitmap, INativeImageFormat format, EncodingOptions options)
    {
        _bitmap = bitmap;
        _format = format;
        _options = options;
    }

    public string FormatNameL10nKey => "FileFormatNameFil";

    public IEnumerable<string> FileNameSuffixes { get; } = [".fil"];

    public ushort LoadAddress { get; set; } = Fil.DefaultLoadAddress;

    public void SaveAs(string fileName)
    {
        var fil = new Fil { Name = Path.GetFileNameWithoutExtension(fileName), Type = new FilType(0x84) };

        var nativeImage = _format.ToNative(new BitmapPixels(_bitmap), _options);
        fil.SetData(nativeImage.Data);
        nativeImage.Metadata?.Embed(fil);

        fil.LoadAddress = LoadAddress;

        using var fs = new FileStream(fileName, FileMode.Create);
        fil.Write(fs);
    }
}
