using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using FilConvWpf.Presenter;
using FilConvWpf.UI;
using ImageLib;

namespace FilConvWpf.Encode
{
    public class NativeEncoding : IEncoding
    {
        private readonly INativeImageFormat _format;
        private readonly bool _canDither;
        private NamedDisplay _currentDisplay;
        private NamedPalette _currentPalette;
        private bool _dither;
        private readonly IMultiChoice<NamedDisplay> _displaySelector;
        private readonly IMultiChoice<NamedPalette> _paletteSelector;
        private readonly IToggle _ditherToggle;

        public event EventHandler<EventArgs> EncodingChanged;

        public NativeEncoding(string name, INativeImageFormat format, bool canDither)
        {
            Name = name;
            _format = format;
            _canDither = canDither;
            _currentDisplay = _displays.First();
            _currentPalette = _palettes.First();

            _displaySelector = new MultiChoiceBuilder<NamedDisplay>()
                .WithCallback(SetCurrentDisplay)
                .Build();

            _paletteSelector = new MultiChoiceBuilder<NamedPalette>()
                .WithCallback(SetCurrentPalette)
                .Build();

            _ditherToggle = new ToggleBuilder()
                .WithIcon("rainbow.png")
                .WithTooltip("ColorDitherToggleTooltip")
                .WithCallback(SetDither)
                .WithInitialState(_dither)
                .Build();

            UpdateTools();
        }

        public string Name { get; }

        public IEnumerable<ITool> Tools { get; private set; }

        public AspectBitmapSource Preview(BitmapSource original)
        {
            return _format.FromNative(ToNative(original), GetDecodingOptions()).ToAspectBitmapSource();
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original)
        {
            return new[] { new FilSaveDelegate(original, _format, GetEncodingOptions()) };
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            if (_format.SupportedDisplays != null)
                settings[EncodingSettingNames.Display] = _currentDisplay;
            if (_format.SupportedPalettes != null)
                settings[EncodingSettingNames.Palette] = _currentPalette;
            if (_canDither)
                settings[EncodingSettingNames.Dithering] = _dither;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            if (_format.SupportedDisplays != null && settings.TryGetValue(EncodingSettingNames.Display, out var d))
            {
                var display = (NamedDisplay)d;
                if (_format.SupportedDisplays.Any(x => x == display.Display))
                {
                    _currentDisplay = display;
                    _displaySelector.CurrentChoice = display;
                }
            }

            if (_format.SupportedPalettes != null && settings.TryGetValue(EncodingSettingNames.Palette, out var p))
            {
                var palette = (NamedPalette)p;
                if (_format.SupportedPalettes.Any(x => x == palette.Palette))
                {
                    _currentPalette = palette;
                    _paletteSelector.CurrentChoice = palette;
                }
            }

            if (_canDither && settings.TryGetValue(EncodingSettingNames.Dithering, out var o))
            {
                _dither = (bool)o;
                _ditherToggle.IsChecked = _dither;
            }
        }

        protected virtual void OnEncodingChanged()
        {
            EncodingChanged?.Invoke(this, EventArgs.Empty);
        }

        private NativeImage ToNative(BitmapSource original)
        {
            return _format.ToNative(new BitmapPixels(original), GetEncodingOptions());
        }

        private void SetCurrentDisplay(NamedDisplay display)
        {
            if (display == _currentDisplay)
            {
                return;
            }

            _currentDisplay = display;
            OnEncodingChanged();
        }

        private void SetCurrentPalette(NamedPalette palette)
        {
            if (palette == _currentPalette)
            {
                return;
            }

            _currentPalette = palette;
            OnEncodingChanged();
        }

        private void SetDither(bool dither)
        {
            if (dither == _dither)
            {
                return;
            }

            _dither = dither;
            OnEncodingChanged();
        }

        private void UpdateTools()
        {
            var tools = new List<ITool>();

            if (_format.SupportedDisplays != null)
            {
                var choices = _displays.Where(x => _format.SupportedDisplays.Contains(x.Display)).ToList();
                _displaySelector.Choices = choices;
                if (choices.Contains(_currentDisplay))
                    _displaySelector.CurrentChoice = _currentDisplay;
                tools.Add(_displaySelector);
            }

            if (_format.SupportedPalettes != null)
            {
                var choices = _palettes.Where(x => _format.SupportedPalettes.Contains(x.Palette)).ToList();
                _paletteSelector.Choices = choices;
                if (choices.Contains(_currentPalette))
                    _paletteSelector.CurrentChoice = _currentPalette;
                tools.Add(_paletteSelector);
            }

            if (_canDither)
                tools.Add(_ditherToggle);

            Tools = tools;
        }

        private EncodingOptions GetEncodingOptions() =>
            new EncodingOptions
            {
                Display = _currentDisplay.Display,
                Palette = _currentPalette.Palette,
                Dither = _dither
            };

        private DecodingOptions GetDecodingOptions() =>
            new DecodingOptions { Display = _currentDisplay.Display, Palette = _currentPalette.Palette };

        private static readonly NamedDisplay[] _displays =
        {
            new NamedDisplay("DisplayNameColor", NativeDisplay.Color),
            new NamedDisplay("DisplayNameMono", NativeDisplay.Mono),
            new NamedDisplay("DisplayNameMonoA7", NativeDisplay.MonoA7),
            new NamedDisplay("DisplayNameMeta", NativeDisplay.Meta),
        };

        private static readonly NamedPalette[] _palettes =
        {
            new NamedPalette("PaletteNameAgat1", NativePalette.Agat1),
            new NamedPalette("PaletteNameAgat2", NativePalette.Agat2),
            new NamedPalette("PaletteNameAgat3", NativePalette.Agat3),
            new NamedPalette("PaletteNameAgat4", NativePalette.Agat4),
        };
    }
}
