using System;
using ImageLib;
using System.Collections.Generic;

namespace FilConvWpf.Native
{
    interface INativeDisplayMode : IToolbarClient
    {
        string Name { get; }
        NativeImageFormat Format { get; }
        double Aspect { get; }
        event EventHandler<EventArgs> FormatChanged;

        void StoreSettings(IDictionary<string, object> settings);
        void AdoptSettings(IDictionary<string, object> settings);
    }
}
