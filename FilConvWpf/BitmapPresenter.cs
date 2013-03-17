using System;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

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

        public AspectBitmap DisplayImage { get; private set; }

        public string[] SupportedPreviewModes
        {
            get
            {
                return null;
            }
        }

        public int PreviewMode
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public ToolBar ToolBar
        {
            get
            {
                return null;
            }
        }
    }
}
