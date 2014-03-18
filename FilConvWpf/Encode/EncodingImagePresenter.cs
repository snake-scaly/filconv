using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ImageLib.Agat;
using ImageLib.Apple;

namespace FilConvWpf.Encode
{
    class EncodingImagePresenter : IImagePresenter
    {
        private const int _defaultEncoding = 2;

        private Preview _sourcePreview;
        private IEncoding _currentEncoding;
        private Dictionary<string, object> _settings;

        public event EventHandler<EventArgs> DisplayImageChanged;

        public EncodingImagePresenter(Preview sourcePreview)
        {
            _sourcePreview = sourcePreview;
            _sourcePreview.DisplayPictureChange += sourcePreview_DisplayPictureChange;

            SupportedPreviewModes = new string[_encodings.Length];
            int i = 0;
            foreach (IEncoding e in _encodings)
            {
                SupportedPreviewModes[i] = e.Name;
                i++;
            }

            _settings = new Dictionary<string, object>();

            PreviewMode = _defaultEncoding;
        }

        public AspectBitmap DisplayImage { get; private set; }

        public string[] SupportedPreviewModes { get; private set; }

        public int PreviewMode
        {
            get
            {
                return Array.IndexOf(_encodings, _currentEncoding);
            }
            set
            {
                if (!object.ReferenceEquals(_currentEncoding, _encodings[value]))
                {
                    if (_currentEncoding != null)
                    {
                        _currentEncoding.StoreSettings(_settings);
                        _currentEncoding.EncodingChanged -= currentEncoding_EncodingChanged;
                    }
                    _currentEncoding = _encodings[value];
                    _currentEncoding.AdoptSettings(_settings);
                    _currentEncoding.EncodingChanged += currentEncoding_EncodingChanged;

                    Encode();
                }
            }
        }

        public ToolBar ToolBar
        {
            get
            {
                return _currentEncoding.ToolBar;
            }
        }

        public IEnumerable<ISaveDelegate> SaveDelegates
        {
            get
            {
                BitmapSource bitmap = _sourcePreview.DisplayPicture;
                return _currentEncoding.GetSaveDelegates(bitmap).Concat(GetStandardSaveDelegates());
            }
        }

        private void currentEncoding_EncodingChanged(object sender, EventArgs e)
        {
            Encode();
        }

        private void sourcePreview_DisplayPictureChange(object sender, EventArgs e)
        {
            Encode();
        }

        private void Encode()
        {
            if (_sourcePreview.DisplayPicture != null)
            {
                DisplayImage = _currentEncoding.Preview(_sourcePreview.DisplayPicture);
            }
            else
            {
                DisplayImage = null;
            }
            OnDisplayImageChanged();
        }

        protected virtual void OnDisplayImageChanged()
        {
            if (DisplayImageChanged != null)
            {
                DisplayImageChanged(this, EventArgs.Empty);
            }
        }

        private IEnumerable<ISaveDelegate> GetStandardSaveDelegates()
        {
            BitmapSource bitmap = DisplayImage.Bitmap;
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameBmp", new string[] { "*.bmp" }, typeof(BmpBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameJpeg", new string[] { "*.jpg", "*,jpeg" }, typeof(JpegBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNamePng", new string[] { "*.png" }, typeof(PngBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameGif", new string[] { "*.gif" }, typeof(GifBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameTiff", new string[] { "*.tif", "*.tiff" }, typeof(TiffBitmapEncoder));
        }

        private static readonly IEncoding[] _encodings =
        {
            new IdentityEncoding(),
            new NativeEncoding("FormatNameGR7", new Gr7ImageFormat()),
            new NativeEncoding("FormatNameMGR", new MgrImageFormat()),
            new NativeEncoding("FormatNameHGR", new HgrImageFormat()),
            new NativeEncoding("FormatNameMGR9", new Mgr9ImageFormat()),
            new NativeEncoding("FormatNameHGR9", new Hgr9ImageFormat()),
            new NativeEncoding("FormatNameApple2LoRes", new Apple2DoubleHiResImageFormat()),
            new AppleHiResEncoding(),
        };
    }
}
