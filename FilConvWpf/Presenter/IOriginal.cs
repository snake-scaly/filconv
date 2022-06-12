using System;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Presenter
{
    public interface IOriginal
    {
        event EventHandler<EventArgs> OriginalChanged;
        BitmapSource OriginalBitmap { get; }
    }
}
