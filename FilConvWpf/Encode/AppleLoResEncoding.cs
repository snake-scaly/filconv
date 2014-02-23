using ImageLib;
using ImageLib.Apple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    class AppleLoResEncoding : IEncoding
    {
        private Apple2LoResImageFormat _format;

        public AppleLoResEncoding()
        {
            _format = new Apple2LoResImageFormat();
        }

        public event EventHandler<EventArgs> EncodingChanged;

        public string Name
        {
            get { return "FormatNameApple2LoRes"; }
        }

        public System.Windows.Controls.ToolBar ToolBar
        {
            get { return null; }
        }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(
                _format.FromNative(_format.ToNative(original, new EncodingOptions())),
                _format.Aspect);
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original)
        {
            yield return new FilSaveDelegate(original, _format, new EncodingOptions());
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
        }
    }
}
