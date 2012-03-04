using System;
using ImageLib;

namespace FilConvWpf.Native
{
    interface INativeDisplayMode : IToolbarClient
    {
        string Name { get; }
        NativeImageFormat Format { get; }
        event EventHandler<EventArgs> FormatChanged;
    }
}
