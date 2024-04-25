using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using FilLib;
using ImageLib;

namespace FilConv.Encode;

class FilSaveDelegate : SaveDelegateAbstr
{
    private readonly Bitmap? _bitmap;
    private readonly INativeImageFormat? _format;
    private readonly EncodingOptions _options;
    private readonly byte[]? _data;
    private readonly string? _fileNameSuffix;

    public FilSaveDelegate(Bitmap bitmap, INativeImageFormat format, EncodingOptions options)
    {
        _bitmap = bitmap;
        _format = format;
        _options = options;
    }

    public FilSaveDelegate(byte[] data, string fileNameSuffix)
    {
        _data = data;
        _fileNameSuffix = fileNameSuffix;
    }

    public override string FormatNameL10nKey => "FileFormatNameFil";

    public override IEnumerable<string> FileNameMasks { get; } = new[] { "*.fil" };

    public ushort LoadAddress { get; set; } = Fil.DefaultLoadAddress;

    public override void SaveAs(string fileName)
    {
        var fil = new Fil { Name = Path.GetFileNameWithoutExtension(fileName), Type = new FilType(0x84) };

        if (_data != null)
        {
            fil.SetData(_data);
        }
        else
        {
            var nativeImage = _format!.ToNative(new BitmapPixels(_bitmap!), _options);
            fil.SetData(nativeImage.Data);
            nativeImage.Metadata?.Embed(fil);
        }

        fil.LoadAddress = LoadAddress;

        using var fs = new FileStream(fileName, FileMode.Create);
        fil.Write(fs);
    }

    protected override string GetBaseName(string fileName)
    {
        fileName = base.GetBaseName(fileName);
        if (_fileNameSuffix != null)
        {
            if (fileName.Length + _fileNameSuffix.Length > Fil.MaxNameLength)
            {
                fileName = fileName.Substring(0, Fil.MaxNameLength - _fileNameSuffix.Length);
            }
            fileName += _fileNameSuffix;
        }
        else if (fileName.Length > Fil.MaxNameLength)
        {
            fileName = fileName.Substring(0, Fil.MaxNameLength);
        }
        return fileName;
    }
}
