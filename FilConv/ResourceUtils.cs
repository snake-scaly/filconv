using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace FilConv;

static class ResourceUtils
{
    public static Image GetResourceImage(string fileName)
    {
        Image result = new Image();
        result.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://FilConv/Resources/{fileName}")));
        return result;
    }
}
