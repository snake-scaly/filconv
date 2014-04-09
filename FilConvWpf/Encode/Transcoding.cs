using ImageLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    class Transcoding : IEncoding
    {
        private NativeImage _nativeImage;
        private NativeImageFormat _format;
        private IEnumerable<ISaveDelegate> _saveDelegates;

        public Transcoding(NativeImage nativeImage, NativeImageFormat format, string nameL10nKey, IEnumerable<ISaveDelegate> saveDelegates)
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

        public string OutputFileNameAddSuffix { get; set; }

        public string OutputFileNameStripSuffix { get; set; }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(_format.FromNative(_nativeImage), _format.Aspect);
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original)
        {
            return _saveDelegates;
        }

        public string DeriveOutputFileName(string inputFileName)
        {
            string name = Path.GetFileNameWithoutExtension(inputFileName);
            if (OutputFileNameStripSuffix != null && name.EndsWith(OutputFileNameStripSuffix))
            {
                name = name.Substring(0, name.Length - OutputFileNameStripSuffix.Length);
            }
            if (OutputFileNameAddSuffix != null)
            {
                name += OutputFileNameAddSuffix;
            }
            return name;
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
        }
    }
}
