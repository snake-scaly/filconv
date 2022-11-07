using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilConvWpf.Encode
{
    abstract class SaveDelegateAbstr : ISaveDelegate
    {
        private const string _piclerFileSuffix = ".bol.fil";

        public abstract string FormatNameL10nKey { get; }

        public abstract IEnumerable<string> FileNameMasks { get; }

        public abstract void SaveAs(string fileName);

        public string DeriveOutputFileName(string inputFileName)
        {
            return GetBaseName(inputFileName) + Path.GetExtension(Enumerable.First(FileNameMasks));
        }

        protected virtual string GetBaseName(string fileName)
        {
            if (fileName.ToLower().EndsWith(_piclerFileSuffix))
            {
                return Path.GetFileName(fileName.Substring(0, fileName.Length - _piclerFileSuffix.Length));
            }
            return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
