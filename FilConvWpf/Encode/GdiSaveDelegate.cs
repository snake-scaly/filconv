using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    class GdiSaveDelegate : SaveDelegateAbstr
    {
        private Type _encoderType;
        private BitmapSource _bitmap;
        private string _name;
        private IEnumerable<string> _masks;

        public GdiSaveDelegate(BitmapSource bitmap, string formatNameL10nKey, IEnumerable<string> fileNameMasks, Type encoderType)
        {
            Debug.Assert(encoderType.IsSubclassOf(typeof(BitmapEncoder)));

            _bitmap = bitmap;
            _name = formatNameL10nKey;
            _masks = new List<string>(fileNameMasks).AsReadOnly();
            _encoderType = encoderType;
        }

        public override string FormatNameL10nKey { get { return _name; } }

        public override IEnumerable<string> FileNameMasks { get { return _masks; } }

        public override void SaveAs(string fileName)
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
