using ImageLib;
using ImageLib.Agat;
using ImageLib.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using FilConvWpf.Presenter;
using FilConvWpf.UI;
using ImageLib.Apple;
using ImageLib.Apple.HiRes;

namespace FilConvWpf.Native
{
    public sealed class NativeImagePresenter : IImagePresenter, INativeOriginal
    {
        private const string _modeSettingsKey = "previewMode";
        private const string _displaySettingsKey = "display";
        private const string _paletteSettingsKey = "palette";

        private readonly IMultiChoice<NamedMode> _modeSelector;
        private readonly IMultiChoice<NamedDisplay> _displaySelector;
        private readonly IMultiChoice<NamedPalette> _paletteSelector;

        private NamedMode _currentMode;
        private NamedDisplay _currentDisplay;
        private NamedPalette _currentPalette;

        public event EventHandler<EventArgs> DisplayImageChanged;
        public event EventHandler<EventArgs> ToolBarChanged;
        public event EventHandler<EventArgs> OriginalChanged;

        public NativeImagePresenter(NativeImage nativeImage)
        {
            NativeImage = nativeImage;
            _currentMode = _modes[0];

            _modeSelector = new MultiChoiceBuilder<NamedMode>()
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
            if (mode.Format.SupportedDisplays != null)
                _currentDisplay = ChooseCompatibleDisplay(null, mode.Format);
            if (mode.Format.SupportedPalettes != null)
                _currentPalette = ChooseCompatiblePalette(null, mode.Format);
            SetCurrentMode(mode);
        }

        public void Dispose()
        {
        }

        public AspectBitmap DisplayImage { get; private set; }

        public BitmapSource OriginalBitmap => DisplayImage.Bitmap;

        public NativeImage NativeImage { get; }

        public INativeImageFormat NativeImageFormat => _currentMode?.Format;

        public IEnumerable<ITool> Tools { get; private set; } = new ITool[] { };

        private void SetCurrentMode(NamedMode mode)
        {
            if (mode == _currentMode)
            {
                return;
            }

            _currentMode = mode;
            _modeSelector.CurrentChoice = mode;

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

        private NamedMode GuessPreviewMode(NamedMode preferred)
        {
            // This relies on OrderByDescending performing a stable sort. Therefore if
            // preferred is among the best it will come out first.
            return new[] { preferred }.Concat(_modes)
                .OrderByDescending(f => f.Format.ComputeMatchScore(NativeImage))
                .First();
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            settings[_modeSettingsKey] = _currentMode;
            if (_currentMode.Format.SupportedDisplays != null)
                settings[_displaySettingsKey] = _currentDisplay;
            if (_currentMode.Format.SupportedPalettes != null)
                settings[_paletteSettingsKey] = _currentPalette;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            if (NativeImage.Metadata != null)
            {
                return;
            }
            if (settings.ContainsKey(_modeSettingsKey))
            {
                SetCurrentMode(GuessPreviewMode((NamedMode)settings[_modeSettingsKey]));
            }
            if (_currentMode.Format.SupportedDisplays != null && settings.ContainsKey(_displaySettingsKey))
            {
                var displaySettings = (NamedDisplay)settings[_displaySettingsKey];
                SetCurrentDisplay(ChooseCompatibleDisplay(displaySettings, _currentMode.Format), true);
            }
            if (_currentMode.Format.SupportedPalettes != null && settings.ContainsKey(_paletteSettingsKey))
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

                _displaySelector.Choices = GetSupportedDisplays();
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

            Tools = tools;
            ToolBarChanged?.Invoke(this, EventArgs.Empty);
        }

        private NamedDisplay ChooseCompatibleDisplay(NamedDisplay current, INativeImageFormat format)
        {
            var supported = GetSupportedDisplays().ToList();
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


        private IEnumerable<NamedDisplay> GetSupportedDisplays()
        {
            var supportedDisplays = _currentMode.Format.SupportedDisplays
                .Select(s => _displays.First(d => d.Display == s));

            if (NativeImage.Metadata == null)
            {
                supportedDisplays = supportedDisplays.Where(s => s.Display != NativeDisplay.Meta);
            }

            return supportedDisplays;
        }

        private static readonly NamedMode[] _modes =
        {
            new NamedMode("FormatNameAgatCGNR", new AgatCGNRImageFormat()),
            new NamedMode("FormatNameAgatCGSR", new AgatCGSRImageFormat()),
            new NamedMode("FormatNameAgatMGVR", new AgatMGVRImageFormat()),
            new NamedMode("FormatNameAgatCGVR", new AgatCGVRImageFormat()),
            new NamedMode("FormatNameAgatMGDP", new AgatMGDPImageFormat()),
            new NamedMode("FormatNameAgatApple", new AgatAppleImageFormat()),
            new NamedMode("FormatNameAgatCGSRDV", new AgatCGSRDVImageFormat()),
            new NamedMode("FormatNameApple2LoRes", new Apple2LoResImageFormat(false)),
            new NamedMode("FormatNameApple2DoubleLoRes", new Apple2LoResImageFormat(true)),
            new NamedMode("FormatNameApple2HiRes", new Apple2HiResImageFormat(new Apple2FillTv(Apple2Palettes.American))),
            new NamedMode("FormatNameApple2DoubleHiRes", new Apple2DoubleHiResImageFormat()),
            new NamedMode("FormatNameSpectrum", new SpectrumImageFormatInterleave()),
            new NamedMode("FormatNamePicler", new SpectrumImageFormatPicler()),
        };

        private static readonly NamedDisplay[] _displays =
        {
            new NamedDisplay("DisplayNameColor", NativeDisplay.Color),
            new NamedDisplay("DisplayNameMono", NativeDisplay.Mono),
            new NamedDisplay("DisplayNameMonoA7", NativeDisplay.MonoA7),
            new NamedDisplay("DisplayNameMeta", NativeDisplay.Meta),
            new NamedDisplay("DisplayNameArtifact", NativeDisplay.Artifact),
        };

        private static readonly NamedPalette[] _palettes =
        {
            new NamedPalette("PaletteNameAgat1", NativePalette.Agat1),
            new NamedPalette("PaletteNameAgat2", NativePalette.Agat2),
            new NamedPalette("PaletteNameAgat3", NativePalette.Agat3),
            new NamedPalette("PaletteNameAgat4", NativePalette.Agat4),
        };

        private class NamedMode : NamedChoice
        {
            public readonly INativeImageFormat Format;

            public NamedMode(string name, INativeImageFormat format) : base(name)
            {
                Format = format;
            }
        }
    }
}
