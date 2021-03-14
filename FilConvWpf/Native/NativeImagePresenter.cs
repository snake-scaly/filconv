using ImageLib;
using ImageLib.Agat;
using ImageLib.Spectrum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Linq;

namespace FilConvWpf.Native
{
    class NativeImagePresenter : IImagePresenter, INativeOriginal
    {
        private const int _defaultMode = 1; // MGR
        private const string _modeSettingsKey = "previewMode";

        private NativeImage _nativeImage;
        private INativeDisplayMode _currentMode;
        private Dictionary<string, object> _settings;
        private int _preferredMode;

        public event EventHandler<EventArgs> DisplayImageChanged;
        public event EventHandler<EventArgs> OriginalChanged;

        public NativeImagePresenter(NativeImage nativeImage)
        {
            _nativeImage = nativeImage;
            _settings = new Dictionary<string, object>();

            _preferredMode = _defaultMode;

            var displayModeNames = new List<string>();
            foreach (INativeDisplayMode m in _displayModes)
            {
                displayModeNames.Add(m.Name);
            }
            SupportedPreviewModes = displayModeNames.ToArray();

            GuessPreviewMode();
        }

        public AspectBitmap DisplayImage { get; private set; }

        public BitmapSource OriginalBitmap
        {
            get { return DisplayImage != null ? DisplayImage.Bitmap : null; }
        }

        public NativeImage NativeImage
        {
            get { return _nativeImage; }
        }

        public INativeImageFormat NativeImageFormat
        {
            get { return _currentMode != null ? _currentMode.Format : null; }
        }

        public string[] SupportedPreviewModes { get; private set; }
        
        public int PreviewMode
        {
            get
            {
                return Array.IndexOf(_displayModes, _currentMode);
            }

            set
            {
                if (value < 0 || value >= _displayModes.Length)
                {
                    value = _defaultMode;
                }

                if (!object.ReferenceEquals(_displayModes[value], _currentMode))
                {
                    if (_currentMode != null)
                    {
                        _currentMode.StoreSettings(_settings);
                        _currentMode.FormatChanged -= currentMode_FormatChanged;
                    }
                    _currentMode = _displayModes[value];
                    _currentMode.FormatChanged += currentMode_FormatChanged;
                }

                _currentMode.AdoptSettings(_settings);
                Convert(_currentMode.Format);
            }
        }

        public ToolBar ToolBar
        {
            get
            {
                return _currentMode.ToolBar;
            }
        }

        private void currentMode_FormatChanged(object sender, EventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, _currentMode));
            Convert(_currentMode.Format);
        }

        private void Convert(INativeImageFormat f)
        {
            DisplayImage = new AspectBitmap(f.FromNative(_nativeImage), _currentMode.Aspect);
            OnDisplayImageChanged();
        }

        protected virtual void OnDisplayImageChanged()
        {
            if (DisplayImageChanged != null)
            {
                DisplayImageChanged(this, EventArgs.Empty);
            }
            if (OriginalChanged != null)
            {
                OriginalChanged(this, EventArgs.Empty);
            }
        }

        private void GuessPreviewMode()
        {
            int bestMode = _preferredMode;
            int bestScore = _displayModes[bestMode].Format.ComputeMatchScore(_nativeImage);
            int i = 0;

            foreach (INativeDisplayMode mode in _displayModes)
            {
                int score = mode.Format.ComputeMatchScore(_nativeImage);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMode = i;
                }
                ++i;
            }

            PreviewMode = bestMode;
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            if (_currentMode != null)
            {
                _currentMode.StoreSettings(settings);
            }
            settings[_modeSettingsKey] = PreviewMode;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            _settings = new Dictionary<string, object>(settings);
            if (settings.ContainsKey(_modeSettingsKey))
            {
                _preferredMode = (int)settings[_modeSettingsKey];
                GuessPreviewMode();
            }
        }

        private readonly INativeDisplayMode[] _displayModes =
        {
            new NativeDisplayMode("FormatNameGR7", new Gr7ImageFormat()),
            new NativeDisplayMode("FormatNameMGR", new MgrImageFormat()),
            new NativeDisplayMode("FormatNameHGR", new HgrImageFormat()),
            new NativeDisplayMode("FormatNameMGR9", new Mgr9ImageFormat()),
            new NativeDisplayMode("FormatNameHGR9", new Hgr9ImageFormat()),
            new NativeDisplayMode("FormatNameDMGR", new DmgrImageFormat()),
            new NativeDisplayMode("FormatNamePicler", new SpectrumImageFormatPicler()),
            new AppleLoResDisplayMode(false),
            new AppleLoResDisplayMode(true),
            new AppleHiResDisplayMode(),
            new AppleDoubleHiResDisplayMode(),
            new NativeDisplayMode("FormatNameSpectrum", new SpectrumImageFormatInterleave()),
        };
    }
}
