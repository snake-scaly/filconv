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
        private bool _dither;
        private readonly IMultiChoice<NamedDisplay> _displaySelector;
        private readonly IToggle _ditherToggle;

        public event EventHandler<EventArgs> EncodingChanged;

        public NativeEncoding(string name, INativeImageFormat format, bool canDither)
        {
            Name = name;
            _format = format;
            _canDither = canDither;
            _currentDisplay = _displays.First();

            _displaySelector = new MultiChoiceBuilder<NamedDisplay>()
                .WithCallback(SetCurrentDisplay)
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

        public AspectBitmap Preview(BitmapSource original)
        {
            return _format.FromNative(ToNative(original), new DecodingOptions { Display = _currentDisplay.Display });
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original)
        {
            yield return new FilSaveDelegate(original, _format, new EncodingOptions { Display = _currentDisplay.Display, Dither = _dither });
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            settings[EncodingSettingNames.Display] = _currentDisplay;
            settings[EncodingSettingNames.Dithering] = _dither;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            if (settings.TryGetValue(EncodingSettingNames.Display, out var d))
            {
                var display = (NamedDisplay)d;
                if (_format.SupportedDisplays.Any(x => x == display.Display))
                {
                    _currentDisplay = display;
                    _displaySelector.CurrentChoice = display;
                }
            }

            if (settings.TryGetValue(EncodingSettingNames.Dithering, out var o))
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
            return _format.ToNative(original, new EncodingOptions { Display = _currentDisplay.Display, Dither = _dither });
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

            if (_canDither)
                tools.Add(_ditherToggle);

            Tools = tools;
        }

        private static readonly NamedDisplay[] _displays =
        {
            new NamedDisplay("DisplayNameColor", NativeDisplay.Color),
            new NamedDisplay("DisplayNameMono", NativeDisplay.Mono),
            new NamedDisplay("DisplayNameMonoA7", NativeDisplay.MonoA7),
        };
    }
}
