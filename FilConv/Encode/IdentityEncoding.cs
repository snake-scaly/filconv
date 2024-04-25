using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using FilConv.UI;

namespace FilConv.Encode;

class IdentityEncoding : IEncoding
{
    public event EventHandler<EventArgs> EncodingChanged
    {
        add { }
        remove { }
    }

    public string Name => "FormatNameOriginal";

    public AspectBitmapSource Preview(Bitmap original)
    {
        return new AspectBitmapSource(original, 1);
    }

    public IEnumerable<ITool> Tools { get; } = new ITool[] { };

    public IEnumerable<ISaveDelegate> GetSaveDelegates(Bitmap bitmap)
    {
        return Enumerable.Empty<ISaveDelegate>();
    }

    public void StoreSettings(IDictionary<string, object> settings)
    {
    }

    public void AdoptSettings(IDictionary<string, object> settings)
    {
    }
}
