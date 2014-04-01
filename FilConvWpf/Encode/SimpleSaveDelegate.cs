using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilConvWpf.Encode
{
    class SimpleSaveDelegate : ISaveDelegate
    {
        private byte[] _data;
        private string _fileNameMask;

        public SimpleSaveDelegate(byte[] data, string formatNameL10nKey, string fileNameMask)
        {
            _data = data;
            FormatNameL10nKey = formatNameL10nKey;
            _fileNameMask = fileNameMask;
        }

        public string FormatNameL10nKey { get; private set; }

        public IEnumerable<string> FileNameMasks
        {
            get { yield return _fileNameMask; }
        }

        public void SaveAs(string fileName)
        {
            File.WriteAllBytes(fileName, _data);
        }
    }
}
