using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
    
namespace FilConvWpf.Encode
{
    class IdentityEncoding : IEncoding
    {
        public event EventHandler<EventArgs> EncodingChanged
        {
            add { }
            remove { }
        }

        public string Name { get { return "FormatNameOriginal"; } }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(original, 1);
        }

        public ToolBar ToolBar
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource bitmap)
        {
            return Enumerable.Empty<ISaveDelegate>();
        }

        public string DeriveOutputFileName(string inputFileName)
        {
            return Path.GetFileNameWithoutExtension(inputFileName);
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
        }
    }
}
