using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using FilConvWpf.UI;
using ImageLib;

namespace FilConvWpf.Encode
{
    public class NativeEncoding : IEncoding
    {
        private INativeImageFormat _format;
        private bool _dither;
        private readonly IToggle _ditherToggle;

        public event EventHandler<EventArgs> EncodingChanged;

        public NativeEncoding(string name, INativeImageFormat format, bool canDither)
        {
            Name = name;
            _format = format;

            if (canDither)
            {
                _ditherToggle = new ToggleBuilder()
                    .WithIcon("rainbow.png")
                    .WithTooltip("ColorDitherToggleTooltip")
                    .WithCallback(on => { _dither = on; OnEncodingChanged(); })
                    .WithInitialState(_dither)
                    .Build();

                Tools = new ITool[] { _ditherToggle };
            }
        }

        public string Name { get; }

        public IEnumerable<ITool> Tools { get; } = new ITool[] { };

        public AspectBitmap Preview(BitmapSource original)
        {
            return _format.FromNative(ToNative(original));
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original)
        {
            yield return new FilSaveDelegate(original, _format, new EncodingOptions { Dither = _dither });
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            settings[SettingNames.Dithering] = _dither;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            if (settings.TryGetValue(SettingNames.Dithering, out var o))
            {
                _dither = (bool)o;
                if (_ditherToggle != null)
                {
                    _ditherToggle.IsChecked = _dither;
                }
            }
        }

        private NativeImage ToNative(BitmapSource original)
        {
            return _format.ToNative(original, new EncodingOptions { Dither = _dither });
        }

        protected virtual void OnEncodingChanged()
        {
            EncodingChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
