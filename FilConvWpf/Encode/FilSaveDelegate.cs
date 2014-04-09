using FilLib;
using ImageLib;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    class FilSaveDelegate : SaveDelegateAbstr
    {
        private BitmapSource _bitmap;
        private NativeImageFormat _format;
        private EncodingOptions _options;
        private byte[] _data;
        private string _fileNameSuffix;

        public FilSaveDelegate(BitmapSource bitmap, NativeImageFormat format, EncodingOptions options)
        {
            _bitmap = bitmap;
            _format = format;
            _options = options;
        }

        public FilSaveDelegate(byte[] data, string fileNameSuffix)
        {
            _data = data;
            _fileNameSuffix = fileNameSuffix;
        }

        public override string FormatNameL10nKey
        {
            get { return "FileFormatNameFil"; }
        }

        public override IEnumerable<string> FileNameMasks
        {
            get
            {
                yield return "*.fil";
            }
        }

        public override void SaveAs(string fileName)
        {
            var fil = new Fil(Path.GetFileNameWithoutExtension(fileName));
            fil.Data = _data ?? _format.ToNative(_bitmap, _options).Data;
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                fil.Write(fs);
            }
        }

        protected override string GetBaseName(string fileName)
        {
            fileName = base.GetBaseName(fileName);
            if (_fileNameSuffix != null)
            {
                fileName += _fileNameSuffix;
            }
            return fileName;
        }
    }
}
