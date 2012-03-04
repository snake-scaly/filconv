using System;
using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    class WpfImageDisplayAdapter : IImageDisplayAdapter
    {
        public event EventHandler<EventArgs> DisplayImageChanged;

        public WpfImageDisplayAdapter(BitmapSource bmp)
        {
            DisplayImage = new DisplayImage(bmp, 1);
        }

        public bool EnableAspectCorrection { get { return false; } }

        public DisplayImage DisplayImage { get; private set; }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
        }

        public void RevokeToolbarFragment()
        {
        }
    }
}
