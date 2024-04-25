using System;
using Avalonia.Media.Imaging;

namespace FilConv.Presenter;

public interface IOriginal
{
    event EventHandler<EventArgs> OriginalChanged;
    Bitmap? OriginalBitmap { get; }
}
