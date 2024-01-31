using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using FilConvWpf.Presenter;
using FilConvWpf.UI;

namespace FilConvWpf.Encode
{
    public sealed class EncodingImagePresenter : IImagePresenter
    {
        private const string _modeSettingsKey = "encodingMode";

        private IOriginal _original;
        private IEncoding _currentEncoding;
        private Dictionary<string, object> _settings;
        private IList<IEncoding> _encodings;
        private IMultiChoice<IEncoding> _encodingSelector;

        public event EventHandler<EventArgs> DisplayImageChanged;
        public event EventHandler<EventArgs> ToolBarChanged;

        public EncodingImagePresenter(IOriginal original)
        {
            _original = original;
            _original.OriginalChanged += original_OriginalChanged;

            _settings = new Dictionary<string, object>();
            UpdateEncodings();
        }

        public void Dispose()
        {
            _original.OriginalChanged -= original_OriginalChanged;
            if (_currentEncoding != null)
            {
                _currentEncoding.EncodingChanged -= currentEncoding_EncodingChanged;
            }
        }

        public AspectBitmapSource DisplayImage { get; private set; }

        public IEnumerable<ITool> Tools { get; private set; } = new ITool[] { };

        public IEnumerable<ISaveDelegate> SaveDelegates =>
            _currentEncoding.GetSaveDelegates(_original.OriginalBitmap).Concat(GetStandardSaveDelegates());

        private void SetCurrentEncoding(IEncoding newEncoding)
        {
            if (newEncoding == _currentEncoding)
            {
                return;
            }

            if (_currentEncoding != null)
            {
                _currentEncoding.StoreSettings(_settings);
                _currentEncoding.EncodingChanged -= currentEncoding_EncodingChanged;
            }

            _currentEncoding = newEncoding;
            _currentEncoding.AdoptSettings(_settings);
            _currentEncoding.EncodingChanged += currentEncoding_EncodingChanged;

            _encodingSelector.CurrentChoice = _currentEncoding;

            UpdateTools();
            Encode();
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
            _encodings = EncodingResolver.GetPossibleEncodings(_original).ToList();

            var newEncoding = GetCompatibleEncoding(_currentEncoding);

            _encodingSelector = new MultiChoiceBuilder<IEncoding>()
                .WithChoices(_encodings, e => e.Name)
                .WithDefaultChoice(newEncoding)
                .WithCallback(SetCurrentEncoding)
                .Build();

            SetCurrentEncoding(newEncoding);
        }

        private IEncoding GetCompatibleEncoding(IEncoding desiredEncoding)
        {
            return _encodings.Contains(desiredEncoding) ? desiredEncoding : _encodings.First();
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

        private void OnDisplayImageChanged()
        {
            DisplayImageChanged?.Invoke(this, EventArgs.Empty);
        }

        private IEnumerable<ISaveDelegate> GetStandardSaveDelegates()
        {
            BitmapSource bitmap = DisplayImage.Bitmap;
            yield return new GdiSaveDelegate(bitmap, "FileFormatNamePng", new[] { "*.png" }, typeof(PngBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameJpeg", new[] { "*.jpg", "*,jpeg" }, typeof(JpegBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameGif", new[] { "*.gif" }, typeof(GifBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameBmp", new[] { "*.bmp" }, typeof(BmpBitmapEncoder));
            yield return new GdiSaveDelegate(bitmap, "FileFormatNameTiff", new[] { "*.tif", "*.tiff" }, typeof(TiffBitmapEncoder));
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            _currentEncoding?.StoreSettings(settings);
            settings[_modeSettingsKey] = _currentEncoding;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            _settings = new Dictionary<string, object>(settings);
            if (settings.ContainsKey(_modeSettingsKey))
            {
                var newEncoding = (IEncoding)settings[_modeSettingsKey];
                SetCurrentEncoding(GetCompatibleEncoding(newEncoding));
            }
        }

        private void UpdateTools()
        {
            Tools = new[] { _encodingSelector }.Concat(_currentEncoding.Tools);
            ToolBarChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
