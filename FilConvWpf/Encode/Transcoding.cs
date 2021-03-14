using ImageLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    class Transcoding : IEncoding
    {
        private NativeImage _nativeImage;
        private INativeImageFormat _format;
        private IEnumerable<ISaveDelegate> _saveDelegates;

        public Transcoding(NativeImage nativeImage, INativeImageFormat format, string nameL10nKey, IEnumerable<ISaveDelegate> saveDelegates)
        {
            _nativeImage = nativeImage;
            _format = format;
            Name = nameL10nKey;
            _saveDelegates = saveDelegates;
        }

        public event EventHandler<EventArgs> EncodingChanged
        {
            add { }
            remove { }
        }

        public string Name { get; private set; }

        public ToolBar ToolBar
        {
            get { return null; }
        }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(_format.FromNative(_nativeImage), _format.Aspect);
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original)
        {
            return _saveDelegates;
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
        }
    }
}
