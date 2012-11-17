using System;

namespace FilConvWpf
{
    interface IImagePresenter : IToolbarClient
    {
        event EventHandler<EventArgs> DisplayImageChanged;
        AspectBitmap DisplayImage { get; }
        bool EnableAspectCorrection { get; }
    }
}
