using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using FilConvWpf.UI;

namespace FilConvWpf.Encode
{
    public interface IEncoding
    {
        event EventHandler<EventArgs> EncodingChanged;

        string Name { get; }
        IEnumerable<ITool> Tools { get; }

        AspectBitmapSource Preview(BitmapSource original);
        IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original);

        void StoreSettings(IDictionary<string, object> settings);
        void AdoptSettings(IDictionary<string, object> settings);
    }
}
