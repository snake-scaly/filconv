using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using FilConvWpf.Presenter;
using FilConvWpf.UI;

namespace FilConvWpf
{
    public sealed class BitmapPresenter : IImagePresenter, IOriginal
    {
        public event EventHandler<EventArgs> DisplayImageChanged
        {
            add { }
            remove { }
        }

        public event EventHandler<EventArgs> ToolBarChanged
        {
            add { }
            remove { }
        }

        public BitmapPresenter(BitmapSource bmp)
        {
            DisplayImage = new AspectBitmapSource(bmp, 1);
        }

        public void Dispose()
        {
        }

        public AspectBitmapSource DisplayImage { get; }

        public IEnumerable<ITool> Tools { get; } = new ITool[] { };

        public event EventHandler<EventArgs> OriginalChanged
        {
            add { }
            remove { }
        }

        public BitmapSource OriginalBitmap => DisplayImage.Bitmap;

        public void StoreSettings(IDictionary<string, object> settings)
        {
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
        }
    }
}
