using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    class GdiSaveDelegate : ISaveDelegate
    {
        private Type _encoderType;
        private BitmapSource _bitmap;

        public GdiSaveDelegate(BitmapSource bitmap, string formatNameL10nKey, IEnumerable<string> fileNameMasks, Type encoderType)
        {
            Debug.Assert(encoderType.IsSubclassOf(typeof(BitmapEncoder)));

            _bitmap = bitmap;
            FormatNameL10nKey = formatNameL10nKey;
            FileNameMasks = new List<string>(fileNameMasks).AsReadOnly();
            _encoderType = encoderType;
        }

        public string FormatNameL10nKey { get; set; }

        public IEnumerable<string> FileNameMasks { get; set; }

        public void SaveAs(string fileName)
        {
            BitmapEncoder encoder = (BitmapEncoder)Activator.CreateInstance(_encoderType);
            encoder.Frames.Add(BitmapFrame.Create(_bitmap));
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }
    }
}
