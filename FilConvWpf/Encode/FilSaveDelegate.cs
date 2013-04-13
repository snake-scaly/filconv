using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib;

namespace FilConvWpf.Encode
{
    class FilSaveDelegate : ISaveDelegate
    {
        private BitmapSource _bitmap;
        private NativeImageFormat _format;
        private EncodingOptions _options;

        public FilSaveDelegate(BitmapSource bitmap, NativeImageFormat format, EncodingOptions options)
        {
            _bitmap = bitmap;
            _format = format;
            _options = options;
        }

        public string FormatNameL10nKey
        {
            get { return "FileFormatNameFil"; }
        }

        public IEnumerable<string> FileNameMasks
        {
            get
            {
                yield return "*.fil";
            }
        }

        public void SaveAs(string fileName)
        {
            var fil = new Fil(Path.GetFileNameWithoutExtension(fileName));
            fil.Data = _format.ToNative(_bitmap, _options).Data;
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                fil.Write(fs);
            }
        }
    }
}
