using ImageLib;
using ImageLib.Agat;
using ImageLib.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using FilConvWpf.UI;

namespace FilConvWpf.Native
{
    class NativeImagePresenter : IImagePresenter, INativeOriginal
    {
        private const string _modeSettingsKey = "previewMode";

        private INativeDisplayMode _currentMode;
        private Dictionary<string, object> _settings;
        private readonly IMultiChoice<INativeDisplayMode> _modeSelector;

        public event EventHandler<EventArgs> DisplayImageChanged;
        public event EventHandler<EventArgs> ToolBarChanged;
        public event EventHandler<EventArgs> OriginalChanged;

        public NativeImagePresenter(NativeImage nativeImage)
        {
            NativeImage = nativeImage;
            _settings = new Dictionary<string, object>();

            GuessPreviewMode(_displayModes.First());

            _modeSelector = new MultiChoiceBuilder<INativeDisplayMode>()
                .WithChoices(_displayModes, m => m.Name)
                .WithDefaultChoice(_currentMode)
                .WithCallback(SetCurrentMode)
                .Build();
            UpdateTools();
        }

        public AspectBitmap DisplayImage { get; private set; }

        public BitmapSource OriginalBitmap => DisplayImage.Bitmap;

        public NativeImage NativeImage { get; }

        public INativeImageFormat NativeImageFormat => _currentMode?.Format;

        public IEnumerable<ITool> Tools { get; private set; } = new ITool[] { };

        private void SetCurrentMode(INativeDisplayMode desiredMode)
        {
            if (desiredMode == _currentMode)
            {
                return;
            }

            if (_currentMode != null)
            {
                _currentMode.StoreSettings(_settings);
                _currentMode.FormatChanged -= currentMode_FormatChanged;
            }

            _currentMode = desiredMode;
            _currentMode.FormatChanged += currentMode_FormatChanged;

            _currentMode.AdoptSettings(_settings);

            UpdateTools();
            Convert(_currentMode.Format);
        }

        private void currentMode_FormatChanged(object sender, EventArgs e)
        {
            Convert(_currentMode.Format);
        }

        private void Convert(INativeImageFormat f)
        {
            DisplayImage = f.FromNative(NativeImage);
            OnDisplayImageChanged();
        }

        private void OnDisplayImageChanged()
        {
            DisplayImageChanged?.Invoke(this, EventArgs.Empty);
            OriginalChanged?.Invoke(this, EventArgs.Empty);
        }

        private void GuessPreviewMode(INativeDisplayMode preferred)
        {
            INativeDisplayMode bestMode = null;
            var bestScore = int.MinValue;

            foreach (INativeDisplayMode mode in _displayModes)
            {
                var score = mode.Format.ComputeMatchScore(NativeImage);
                if (score > bestScore || (mode == preferred && score == bestScore))
                {
                    bestScore = score;
                    bestMode = mode;
                }
            }

            SetCurrentMode(bestMode);
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            _currentMode?.StoreSettings(settings);
            settings[_modeSettingsKey] = _currentMode;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            _settings = new Dictionary<string, object>(settings);
            if (settings.ContainsKey(_modeSettingsKey))
            {
                GuessPreviewMode((INativeDisplayMode)settings[_modeSettingsKey]);
            }
        }

        private void UpdateTools()
        {
            Tools = new[] { _modeSelector }.Concat(_currentMode.Tools);
            ToolBarChanged?.Invoke(this, EventArgs.Empty);
        }

        private static readonly INativeDisplayMode[] _displayModes =
        {
            new NativeDisplayMode("FormatNameAgatCGNR", new AgatCGNRImageFormat()),
            new NativeDisplayMode("FormatNameAgatCGSR", new AgatCGSRImageFormat()),
            new NativeDisplayMode("FormatNameAgatMGVR", new AgatMGVRImageFormat()),
            new NativeDisplayMode("FormatNameAgatCGVR", new AgatCGVRImageFormat()),
            new NativeDisplayMode("FormatNameAgatMGDP", new AgatMGDPImageFormat()),
            new NativeDisplayMode("FormatNameAgatCGSRDV", new AgatCGSRDVImageFormat()),
            new AppleLoResDisplayMode(false),
            new AppleLoResDisplayMode(true),
            new AppleHiResDisplayMode(),
            new AppleDoubleHiResDisplayMode(),
            new NativeDisplayMode("FormatNameSpectrum", new SpectrumImageFormatInterleave()),
            new NativeDisplayMode("FormatNamePicler", new SpectrumImageFormatPicler()),
        };
    }
}
