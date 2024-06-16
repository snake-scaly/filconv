using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using FilConv.UI;

namespace FilConv.Encode;

public interface IEncoding
{
    event EventHandler<EventArgs> EncodingChanged;

    string Name { get; }
    IEnumerable<ITool> Tools { get; }

    AspectBitmapSource Preview(Bitmap original);
    IEnumerable<ISaveDelegate> GetSaveDelegates(Bitmap original);
    ISaveDelegate? GetRawSaveDelegate(Bitmap original);

    void StoreSettings(IDictionary<string, object> settings);
    void AdoptSettings(IDictionary<string, object> settings);
}
