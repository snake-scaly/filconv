using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    interface IImageDisplayAdapter : IDisplayImageProvider, IToolbarClient
    {
        bool EnableAspectCorrection { get; }
    }
}
