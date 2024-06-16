using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using FilConv.Native;
using FilConv.Presenter;
using FilLib;
using ImageLib;

namespace FilConv.Services;

public class ImageFileService : IImageFileService
{
    public async Task<IImagePresenter> LoadAsync(string fileName)
    {
        if (fileName.EndsWith(".fil", StringComparison.InvariantCultureIgnoreCase))
        {
            var fil = Fil.FromFile(fileName);
            ImageMeta.TryParse(fil, out var metadata);

            var ni = new NativeImage
            {
                Data = fil.GetData(),
                Metadata = metadata,
                FormatHint = new FormatHint(fileName),
            };

            return new NativeImagePresenter(ni);
        }

        if (fileName.EndsWith(".scr", StringComparison.InvariantCultureIgnoreCase))
            return await LoadRawAsync(fileName);

        var bmp = new Bitmap(fileName);
        _ = bmp.Format ?? throw new NotSupportedException("Unsupported image format");
        return new BitmapPresenter(bmp);
    }

    public async Task<IImagePresenter> LoadRawAsync(string fileName)
    {
        NativeImage ni = new NativeImage
        {
            Data = await File.ReadAllBytesAsync(fileName),
            FormatHint = new FormatHint(fileName)
        };

        return new NativeImagePresenter(ni);
    }
}
