using System;
using ImageLib;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FilConvWpf.Native
{
    interface INativeDisplayMode
    {
        string Name { get; }
        INativeImageFormat Format { get; }
        double Aspect { get; }
        event EventHandler<EventArgs> FormatChanged;
        ToolBar ToolBar { get; }

        void StoreSettings(IDictionary<string, object> settings);
        void AdoptSettings(IDictionary<string, object> settings);
    }
}
