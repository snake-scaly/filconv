using System;
using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    interface IOriginal
    {
        event EventHandler<EventArgs> OriginalChanged;
        BitmapSource OriginalBitmap { get; }
    }
}
