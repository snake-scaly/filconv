using System;
using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    class BitmapPresenter : IImagePresenter
    {
        public event EventHandler<EventArgs> DisplayImageChanged
        {
            add { }
            remove { }
        }

        public BitmapPresenter(BitmapSource bmp)
        {
            DisplayImage = new AspectBitmap(bmp, 1);
        }

        public bool EnableAspectCorrection { get { return false; } }

        public AspectBitmap DisplayImage { get; private set; }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
        }

        public void RevokeToolbarFragment()
        {
        }
    }
}
