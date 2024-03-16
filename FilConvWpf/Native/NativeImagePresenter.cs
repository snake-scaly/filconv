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
        private readonly ITool[] _tools;

        private NamedMode _currentMode;
        private NamedDisplay _currentDisplay;
        private NamedPalette _currentPalette;

        private int _updateDisplayImageSemaphore;
        private int _toolBarChangedSemaphore;

        public event EventHandler<EventArgs> DisplayImageChanged;
        public event EventHandler<EventArgs> ToolBarChanged;
        public event EventHandler<EventArgs> OriginalChanged;

        public NativeImagePresenter(NativeImage nativeImage)
        {
            NativeImage = nativeImage;

            _modeSelector = new MultiChoiceBuilder<NamedMode>()
                .WithChoices(_modes, m => m.Name)
                .WithCallback(ModeSelector_Callback)
                .Build();

            _displaySelector = new MultiChoiceBuilder<NamedDisplay>()
                .WithCallback(DisplaySelector_Callback)
                .Build();

            _paletteSelector = new MultiChoiceBuilder<NamedPalette>()
                .WithCallback(PaletteSelector_Callback)
                .Build();

            _tools = new ITool[] { _modeSelector, _displaySelector, _paletteSelector };

            _modeSelector.CurrentChoice = GuessPreviewMode(_modes.First());
        }

        public void Dispose()
        {
        }

        public AspectBitmapSource DisplayImage { get; private set; }

        public BitmapSource OriginalBitmap => DisplayImage.Bitmap;

        public NativeImage NativeImage { get; }

        public INativeImageFormat NativeImageFormat => _currentMode?.Format;

        public IEnumerable<ITool> Tools => _tools.Where(x => x.Element.IsEnabled);

        public void StoreSettings(IDictionary<string, object> settings)
        {
            settings[_modeSettingsKey] = _currentMode;
            if (_currentMode.Format.SupportedDisplays != null)
            {
                settings[_displaySettingsKey] = _currentDisplay;
                if (_currentMode.Format.GetSupportedPalettes(_currentDisplay.Display) != null)
                    settings[_paletteSettingsKey] = _currentPalette;
            }
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            if (NativeImage.Metadata != null)
            {
                return;
            }

            _updateDisplayImageSemaphore++;
            _toolBarChangedSemaphore++;

            if (settings.TryGetValue(_modeSettingsKey, out var m))
            {
                _modeSelector.CurrentChoice = GuessPreviewMode((NamedMode)m);
            }
            if (_displaySelector.Element.IsEnabled && settings.TryGetValue(_displaySettingsKey, out var d))
            {
                var displaySettings = (NamedDisplay)d;
                _displaySelector.CurrentChoice = ChooseCompatibleDisplay(displaySettings, _currentMode.Format);
            }
            if (_paletteSelector.Element.IsEnabled && settings.TryGetValue(_paletteSettingsKey, out var p))
            {
                var paletteSettings = (NamedPalette)p;
                _paletteSelector.CurrentChoice = ChooseCompatiblePalette(paletteSettings, _currentMode.Format);
            }

            _toolBarChangedSemaphore--;
            OnToolBarChanged();

            _updateDisplayImageSemaphore--;
            UpdateDisplayImage();
        }

        private void ModeSelector_Callback(NamedMode mode)
        {
            if (mode == _currentMode)
                return;

            _currentMode = mode;

            _updateDisplayImageSemaphore++;
            _toolBarChangedSemaphore++;

            UpdateDisplayTool();

            _toolBarChangedSemaphore--;
            OnToolBarChanged();

            _updateDisplayImageSemaphore--;
            UpdateDisplayImage();
        }

        private void DisplaySelector_Callback(NamedDisplay display)
        {
            if (display == _currentDisplay)
                return;

            _currentDisplay = display;

            _updateDisplayImageSemaphore++;
            _toolBarChangedSemaphore++;

            UpdatePaletteTool();

            _toolBarChangedSemaphore--;
            OnToolBarChanged();

            _updateDisplayImageSemaphore--;
            UpdateDisplayImage();
        }

        private void PaletteSelector_Callback(NamedPalette palette)
        {
            if (palette == _currentPalette)
                return;

            _currentPalette = palette;
            UpdateDisplayImage();
        }

        private void UpdateDisplayTool()
        {
            var format = _currentMode.Format;

            if (format.SupportedDisplays == null)
            {
                _displaySelector.Element.IsEnabled = false;
                _paletteSelector.Element.IsEnabled = false;
                return;
            }

            var display = ChooseCompatibleDisplay(_currentDisplay, format);
            _displaySelector.Choices = GetSupportedDisplays(format);
            _displaySelector.CurrentChoice = display;
            _displaySelector.Element.IsEnabled = true;
        }

        private void UpdatePaletteTool()
        {
            if (_currentDisplay == null)
            {
                _paletteSelector.Element.IsEnabled = false;
                return;
            }

            var format = _currentMode.Format;

            var supportedPalettes = format.GetSupportedPalettes(_currentDisplay.Display)?
                .Select(p => _palettes.First(s => s.Palette == p));

            if (supportedPalettes == null)
            {
                _paletteSelector.Element.IsEnabled = false;
                return;
            }

            var palette = ChooseCompatiblePalette(_currentPalette, format);
            _paletteSelector.Choices = supportedPalettes;
            _paletteSelector.CurrentChoice = palette;
            _paletteSelector.Element.IsEnabled = true;
        }

        private void UpdateDisplayImage()
        {
            if (_updateDisplayImageSemaphore < 0)
                throw new Exception($"{nameof(_updateDisplayImageSemaphore)} below zero");
            if (_updateDisplayImageSemaphore > 0)
                return;

            var options = new DecodingOptions
            {
                Display = _currentDisplay?.Display ?? NativeDisplay.Color,
                Palette = _currentPalette?.Palette ?? NativePalette.Default,
            };

            DisplayImage = _currentMode.Format.FromNative(NativeImage, options).ToAspectBitmapSource();
            OnDisplayImageChanged();
        }

        private void OnDisplayImageChanged()
        {
            DisplayImageChanged?.Invoke(this, EventArgs.Empty);
            OriginalChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnToolBarChanged()
        {
            if (_toolBarChangedSemaphore < 0)
                throw new Exception($"{nameof(_toolBarChangedSemaphore)} below zero");
            if (_toolBarChangedSemaphore > 0)
                return;
            ToolBarChanged?.Invoke(this, EventArgs.Empty);
        }

        private NamedMode GuessPreviewMode(NamedMode preferred)
        {
            // This relies on OrderByDescending performing a stable sort. Therefore if
            // preferred is among the best it will come out first.
            return new[] { preferred }.Concat(_modes)
                .OrderByDescending(f => f.Format.ComputeMatchScore(NativeImage))
                .First();
        }

        private NamedDisplay ChooseCompatibleDisplay(NamedDisplay current, INativeImageFormat format)
        {
            if (format.SupportedDisplays == null)
                return current;
            var supported = GetSupportedDisplays(format).ToList();
            if (supported.Contains(current))
            {
                return current;
            }
            var defaultDisplay = format.GetDefaultDecodingOptions(NativeImage).Display;
            return supported.FirstOrDefault(d => d.Display == defaultDisplay) ?? supported.First();
        }

        private NamedPalette ChooseCompatiblePalette(NamedPalette current, INativeImageFormat format)
        {
            if (_currentDisplay == null)
                return current;
            var supportedPalettes = format.GetSupportedPalettes(_currentDisplay.Display)?.ToList();
            if (supportedPalettes == null || (current != null && supportedPalettes.Contains(current.Palette)))
            {
                return current;
            }
            var defaultPalette = format.GetDefaultDecodingOptions(NativeImage).Palette;
            return _palettes.First(s => s.Palette == defaultPalette);
        }

        private IEnumerable<NamedDisplay> GetSupportedDisplays(INativeImageFormat format)
        {
            var supportedDisplays = format.SupportedDisplays
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
            new NamedMode("FormatNameApple2HiRes", new Apple2HiResImageFormat()),
            new NamedMode("FormatNameApple2DoubleHiRes", new Apple2DoubleHiResImageFormat()),
            new NamedMode("FormatNameSpectrum", new SpectrumImageFormatInterleave()),
        };

        private static readonly NamedDisplay[] _displays =
        {
            new NamedDisplay("DisplayNameColor", NativeDisplay.Color),
            new NamedDisplay("DisplayNameColorFilled", NativeDisplay.ColorFilled),
            new NamedDisplay("DisplayNameColorStriped", NativeDisplay.ColorStriped),
            new NamedDisplay("DisplayNameMono", NativeDisplay.Mono),
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
