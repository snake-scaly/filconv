using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    class BitmapPresenter : IImagePresenter, IOriginal
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

        public event EventHandler<EventArgs> OriginalChanged
        {
            add { }
            remove { }
        }

        public BitmapSource OriginalBitmap
        {
            get { return DisplayImage.Bitmap; }
        }
    }
}
