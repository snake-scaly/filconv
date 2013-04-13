using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    interface IEncoding
    {
        event EventHandler<EventArgs> EncodingChanged;

        string Name { get; }
        ToolBar ToolBar { get; }

        AspectBitmap Preview(BitmapSource original);
        IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original);

        void StoreSettings(IDictionary<string, object> settings);
        void AdoptSettings(IDictionary<string, object> settings);
    }
}
