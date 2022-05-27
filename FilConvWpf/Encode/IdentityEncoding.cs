using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using FilConvWpf.UI;
using ImageLib;

namespace FilConvWpf.Encode
{
    class IdentityEncoding : IEncoding
    {
        public event EventHandler<EventArgs> EncodingChanged
        {
            add { }
            remove { }
        }

        public string Name => "FormatNameOriginal";

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(original, 1);
        }

        public IEnumerable<ITool> Tools { get; } = new ITool[] { };

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource bitmap)
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
}
