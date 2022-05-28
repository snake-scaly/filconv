using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using FilConvWpf.UI;
using ImageLib;

namespace FilConvWpf.Encode
{
    public class Transcoding : IEncoding
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

        public string Name { get; }

        public IEnumerable<ITool> Tools { get; } = new ITool[] { };

        public AspectBitmap Preview(BitmapSource original)
        {
            return _format.FromNative(_nativeImage, new DecodingOptions());
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
