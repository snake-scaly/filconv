using System;
using ImageLib;
using System.Collections.Generic;
using FilConvWpf.UI;

namespace FilConvWpf.Native
{
    interface INativeDisplayMode
    {
        event EventHandler<EventArgs> FormatChanged;

        string Name { get; }
        INativeImageFormat Format { get; }
        double Aspect { get; }
        IEnumerable<ITool> Tools { get; }

        void StoreSettings(IDictionary<string, object> settings);
        void AdoptSettings(IDictionary<string, object> settings);
    }
}
