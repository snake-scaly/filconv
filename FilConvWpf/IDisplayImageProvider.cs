using System;

namespace FilConvWpf
{
    interface IDisplayImageProvider
    {
        DisplayImage DisplayImage { get; }
        event EventHandler<EventArgs> DisplayImageChanged;
    }
}
