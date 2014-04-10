using ImageLib.Agat;
using ImageLib.Apple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    class EncodingImagePresenter : IImagePresenter
    {
        private const int _defaultEncoding = 0;
        private const string _modeSettingsKey = "encodingMode";

        private IOriginal _original;
        private IEncoding _currentEncoding;
        private Dictionary<string, object> _settings;
        private IList<IEncoding> _encodings;

        public event EventHandler<EventArgs> DisplayImageChanged;

        public EncodingImagePresenter(IOriginal original)
        {
            _original = original;
            _original.OriginalChanged += original_OriginalChanged;

            _settings = new Dictionary<string, object>();
            UpdateEncodings();
        }

        public AspectBitmap DisplayImage { get; private set; }

        public string[] SupportedPreviewModes { get; private set; }

        public int PreviewMode
        {
            get
            {
                return _encodings.IndexOf(_currentEncoding);
            }
            set
            {
                if (value >= _encodings.Count)
                {
                    value = _defaultEncoding;
                }
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
                BitmapSource bitmap = _original.OriginalBitmap;
                return _currentEncoding.GetSaveDelegates(bitmap).Concat(GetStandardSaveDelegates());
            }
        }

        private void currentEncoding_EncodingChanged(object sender, EventArgs e)
        {
            Encode();
        }

        private void original_OriginalChanged(object sender, EventArgs e)
        {
            UpdateEncodings();
            Encode();
        }

        private void UpdateEncodings()
        {
            int mode = _encodings != null ? PreviewMode : _defaultEncoding;
            _encodings = EncodingResolutionService.GetPossibleEncodings(_original).ToList();
            SupportedPreviewModes = _encodings.Select(x => x.Name).ToArray();
            PreviewMode = mode;
        }

        private void Encode()
        {
            if (_original.OriginalBitmap != null)
            {
                DisplayImage = _currentEncoding.Preview(_original.OriginalBitmap);
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
            yield return new GdiSaveDelegate(bitmap, "FileFormatNamePng", new string[] { "*.png" }, typeof(PngBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameJpeg", new string[] { "*.jpg", "*,jpeg" }, typeof(JpegBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameGif", new string[] { "*.gif" }, typeof(GifBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameBmp", new string[] { "*.bmp" }, typeof(BmpBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameTiff", new string[] { "*.tif", "*.tiff" }, typeof(TiffBitmapEncoder));
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            if (_currentEncoding != null)
            {
                _currentEncoding.StoreSettings(settings);
            }
            settings[_modeSettingsKey] = PreviewMode;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            _settings = new Dictionary<string, object>(settings);
            if (settings.ContainsKey(_modeSettingsKey))
            {
                PreviewMode = (int)settings[_modeSettingsKey];
            }
        }
    }
}
