using FilLib;
using ImageLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    class FilSaveDelegate : SaveDelegateAbstr
    {
        private BitmapSource _bitmap;
        private INativeImageFormat _format;
        private EncodingOptions _options;
        private byte[] _data;
        private string _fileNameSuffix;

        private FilSaveDelegate()
        {
            StartAddress = Fil.DefauldLoadingAddress;
        }

        public FilSaveDelegate(BitmapSource bitmap, INativeImageFormat format, EncodingOptions options)
            : this()
        {
            _bitmap = bitmap;
            _format = format;
            _options = options;
        }

        public FilSaveDelegate(byte[] data, string fileNameSuffix)
            : this()
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

        public UInt16 StartAddress { get; set; }

        public override void SaveAs(string fileName)
        {
            var fil = new Fil(Path.GetFileNameWithoutExtension(fileName));
            fil.Data = _data ?? _format.ToNative(_bitmap, _options).Data;
            fil.StartAddress = StartAddress;
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
                if (fileName.Length + _fileNameSuffix.Length > Fil.MaxNameLength)
                {
                    fileName = fileName.Substring(0, Fil.MaxNameLength - _fileNameSuffix.Length);
                }
                fileName += _fileNameSuffix;
            }
            else if (fileName.Length > Fil.MaxNameLength)
            {
                fileName = fileName.Substring(0, Fil.MaxNameLength);
            }
            return fileName;
        }
    }
}
