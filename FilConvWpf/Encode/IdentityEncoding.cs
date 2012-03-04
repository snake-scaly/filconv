using System;
using System.Windows.Media.Imaging;
    
namespace FilConvWpf.Encode
{
    class IdentityEncoding : IEncoding
    {
        public event EventHandler<EventArgs> EncodingChanged;

        public string Name { get { return "Оригинал"; } }

        public DisplayImage Encode(BitmapSource original)
        {
            return new DisplayImage(original, 1);
        }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
        }

        public void RevokeToolbarFragment()
        {
        }

        public bool IsContainerSupported(Type type)
        {
            return false;
        }

        public void FillContainerData(object container, BitmapSource original)
        {
            throw new NotSupportedException("Containers are not supported");
        }
    }
}
