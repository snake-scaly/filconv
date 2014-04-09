using System.Collections.Generic;
using System.IO;

namespace FilConvWpf.Encode
{
    class SimpleSaveDelegate : SaveDelegateAbstr
    {
        private byte[] _data;
        private string _name;
        private string _fileNameMask;

        public SimpleSaveDelegate(byte[] data, string formatNameL10nKey, string fileNameMask)
        {
            _data = data;
            _name = formatNameL10nKey;
            _fileNameMask = fileNameMask;
        }

        public override string FormatNameL10nKey { get { return _name; } }

        public override IEnumerable<string> FileNameMasks
        {
            get { yield return _fileNameMask; }
        }

        public override void SaveAs(string fileName)
        {
            File.WriteAllBytes(fileName, _data);
        }
    }
}
