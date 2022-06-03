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
        private const string _displaySettingsKey = "display";
        private const string _paletteSettingsKey = "palette";

        private readonly IMultiChoice<INativeDisplayMode> _modeSelector;
        private readonly IMultiChoice<NamedDisplay> _displaySelector;
        private readonly IMultiChoice<NamedPalette> _paletteSelector;

        private INativeDisplayMode _currentMode;
        private NamedDisplay _currentDisplay;
        private NamedPalette _currentPalette;
        private Dictionary<string, object> _settings;

        public event EventHandler<EventArgs> DisplayImageChanged;
        public event EventHandler<EventArgs> ToolBarChanged;
        public event EventHandler<EventArgs> OriginalChanged;

        public NativeImagePresenter(NativeImage nativeImage)
        {
            NativeImage = nativeImage;
            _settings = new Dictionary<string, object>();
            _currentMode = _modes[0];

            _modeSelector = new MultiChoiceBuilder<INativeDisplayMode>()
                .WithChoices(_modes, m => m.Name)
                .WithDefaultChoice(_currentMode)
                .WithCallback(SetCurrentMode)
                .Build();

            _displaySelector = new MultiChoiceBuilder<NamedDisplay>()
                .WithCallback(d => SetCurrentDisplay(d, true))
                .Build();

            _paletteSelector = new MultiChoiceBuilder<NamedPalette>()
                .WithCallback(p => SetCurrentPalette(p, true))
                .Build();

            var mode = GuessPreviewMode(_modes.First());
            _currentDisplay = ChooseCompatibleDisplay(null, mode.Format);
            _currentPalette = ChooseCompatiblePalette(null, mode.Format);
            SetCurrentMode(mode);
        }

        public AspectBitmap DisplayImage { get; private set; }

        public BitmapSource OriginalBitmap => DisplayImage.Bitmap;

        public NativeImage NativeImage { get; }

        public INativeImageFormat NativeImageFormat => _currentMode?.Format;

        public IEnumerable<ITool> Tools { get; private set; } = new ITool[] { };

        private void SetCurrentMode(INativeDisplayMode mode)
        {
            if (mode == _currentMode)
            {
                return;
            }

            if (_currentMode != null)
            {
                _currentMode.StoreSettings(_settings);
                _currentMode.FormatChanged -= currentMode_FormatChanged;
            }

            _currentMode = mode;
            _currentMode.FormatChanged += currentMode_FormatChanged;
            _currentMode.AdoptSettings(_settings);

            _modeSelector.CurrentChoice = _currentMode;

            UpdateTools();
            UpdateDisplayImage();
        }

        private void SetCurrentDisplay(NamedDisplay display, bool updateDisplay)
        {
            if (display == _currentDisplay)
            {
                return;
            }

            _currentDisplay = display;
            _displaySelector.CurrentChoice = display;

            if (updateDisplay)
            {
                UpdateDisplayImage();
            }
        }

        private void SetCurrentPalette(NamedPalette palette, bool updateDisplay)
        {
            if (palette == _currentPalette)
            {
                return;
            }

            _currentPalette = palette;
            _paletteSelector.CurrentChoice = palette;

            if (updateDisplay)
            {
                UpdateDisplayImage();
            }
        }

        private void currentMode_FormatChanged(object sender, EventArgs e)
        {
            UpdateTools();
            UpdateDisplayImage();
        }

        private void UpdateDisplayImage()
        {
            var options = new DecodingOptions
            {
                Display = _currentDisplay?.Display ?? NativeDisplay.Color,
                Palette = _currentPalette?.Palette ?? NativePalette.Default,
            };
            DisplayImage = _currentMode.Format.FromNative(NativeImage, options);
            OnDisplayImageChanged();
        }

        private void OnDisplayImageChanged()
        {
            DisplayImageChanged?.Invoke(this, EventArgs.Empty);
            OriginalChanged?.Invoke(this, EventArgs.Empty);
        }

        private INativeDisplayMode GuessPreviewMode(INativeDisplayMode preferred)
        {
            // This relies on OrderByDescending performing a stable sort. Therefore if
            // preferred is among the best it will come out first.
            return new[] { preferred }.Concat(_modes)
                .OrderByDescending(f => f.Format.ComputeMatchScore(NativeImage))
                .First();
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            _currentMode?.StoreSettings(settings);
            settings[_modeSettingsKey] = _currentMode;
            settings[_displaySettingsKey] = _currentDisplay;
            settings[_paletteSettingsKey] = _currentPalette;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            if (NativeImage.Metadata != null)
            {
                return;
            }

            _settings = new Dictionary<string, object>(settings);
            if (settings.ContainsKey(_modeSettingsKey))
            {
                SetCurrentMode(GuessPreviewMode((INativeDisplayMode)settings[_modeSettingsKey]));
            }
            if (settings.ContainsKey(_displaySettingsKey))
            {
                var displaySettings = (NamedDisplay)settings[_displaySettingsKey];
                SetCurrentDisplay(ChooseCompatibleDisplay(displaySettings, _currentMode.Format), true);
            }
            if (settings.ContainsKey(_paletteSettingsKey))
            {
                var paletteSettings = (NamedPalette)settings[_paletteSettingsKey];
                SetCurrentPalette(ChooseCompatiblePalette(paletteSettings, _currentMode.Format), true);
            }
        }

        private void UpdateTools()
        {
            IEnumerable<ITool> tools = new[] { _modeSelector };

            var format = _currentMode.Format;

            if (format.SupportedDisplays != null)
            {
                var display = ChooseCompatibleDisplay(_currentDisplay, format);

                _displaySelector.Choices = _displays;
                _displaySelector.CurrentChoice = display;

                SetCurrentDisplay(display, false);

                tools = tools.Concat(new[] { _displaySelector });
            }

            if (format.SupportedPalettes != null)
            {
                var supportedPalettes = format.SupportedPalettes
                    .Select(p => _palettes.First(s => s.Palette == p));

                var palette = ChooseCompatiblePalette(_currentPalette, format);

                _paletteSelector.Choices = supportedPalettes;
                _paletteSelector.CurrentChoice = palette;

                SetCurrentPalette(palette, false);

                tools = tools.Concat(new[] { _paletteSelector });
            }

            Tools = tools.Concat(_currentMode.Tools);
            ToolBarChanged?.Invoke(this, EventArgs.Empty);
        }

        private NamedDisplay ChooseCompatibleDisplay(NamedDisplay current, INativeImageFormat format)
        {
            var supported = _displays.ToList();
            if (current != null && supported.Contains(current))
            {
                return current;
            }
            var defaultDisplay = format.GetDefaultDecodingOptions(NativeImage).Display;
            return supported.FirstOrDefault(d => d.Display == defaultDisplay) ?? supported.First();
        }

        private NamedPalette ChooseCompatiblePalette(NamedPalette current, INativeImageFormat format)
        {
            if (format?.SupportedPalettes == null ||
                current != null && format.SupportedPalettes.Contains(current.Palette))
            {
                return current;
            }
            var defaultPalette = format.GetDefaultDecodingOptions(NativeImage).Palette;
            return _palettes.First(s => s.Palette == defaultPalette);
        }

        private static readonly INativeDisplayMode[] _modes =
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

        private static readonly NamedDisplay[] _allDisplays =
        {
            new NamedDisplay("DisplayNameColor", NativeDisplay.Color),
            new NamedDisplay("DisplayNameMono", NativeDisplay.Mono),
            new NamedDisplay("DisplayNameMonoA7", NativeDisplay.MonoA7),
            new NamedDisplay("DisplayNameMeta", NativeDisplay.Meta),
        };

        private IList<NamedDisplay> _displays
        {
            get
            {
                return NativeImage.Metadata != null
                    ? (IList<NamedDisplay>)_allDisplays
                    : _allDisplays.Where(x => x.Display != NativeDisplay.Meta).ToList();
            }
        }

        private static readonly NamedPalette[] _palettes =
        {
            new NamedPalette("PaletteNameAgat1", NativePalette.Agat1),
            new NamedPalette("PaletteNameAgat2", NativePalette.Agat2),
            new NamedPalette("PaletteNameAgat3", NativePalette.Agat3),
            new NamedPalette("PaletteNameAgat4", NativePalette.Agat4),
        };

        private class Named
        {
            public readonly string Name;

            protected Named(string name)
            {
                Name = name;
            }

            public override string ToString() => Name;
        }

        private class NamedDisplay : Named
        {
            public readonly NativeDisplay Display;

            public NamedDisplay(string name, NativeDisplay display)
                : base(name)
            {
                Display = display;
            }
        };

        private class NamedPalette : Named
        {
            public readonly NativePalette Palette;

            public NamedPalette(string name, NativePalette palette)
                : base(name)
            {
                Palette = palette;
            }
        }
    }
}
