using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using FilConvWpf.UI;
using ImageLib;
using ImageLib.Apple;
using ImageLib.Util;

namespace FilConvWpf.Encode
{
    class AppleHiResEncoding : IEncoding
    {
        private bool _fill;
        private bool _pal;

        private readonly IToggle _fillToggle;
        private readonly IToggle _palToggle;

        private Apple2HiResImageFormat _format;

        public AppleHiResEncoding()
        {
            _fillToggle = new ToggleBuilder()
                .WithIcon("fill.png")
                .WithTooltip("Apple2ColorFillToggleTooltip")
                .WithCallback( on => { _fill = on; UpdateFormat(); })
                .WithInitialState(_fill)
                .Build();

            _palToggle = new ToggleBuilder()
                .WithIcon("useu.png")
                .WithTooltip("Apple2PaletteToggleTooltip")
                .WithCallback( on => { _pal = on; UpdateFormat(); })
                .WithInitialState(_pal)
                .Build();

            Tools = new ITool[] { _fillToggle, _palToggle };

            _format = new Apple2HiResImageFormat(new Apple2SimpleTv(Apple2Palettes.European));
        }

        public event EventHandler<EventArgs> EncodingChanged;

        public string Name => "FormatNameApple2HiRes";

        public IEnumerable<ITool> Tools { get; }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(
                _format.FromNative(_format.ToNative(original, new EncodingOptions())),
                _format.Aspect);
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original)
        {
            yield return new FilSaveDelegate(original, _format, new EncodingOptions());
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            settings[SettingNames.AppleFill] = _fill;
            settings[SettingNames.ApplePalette] = _pal;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            object o;

            if (settings.TryGetValue(SettingNames.AppleFill, out o))
            {
                _fill = (bool)o;
                _fillToggle.IsChecked = _fill;
            }

            if (settings.TryGetValue(SettingNames.ApplePalette, out o))
            {
                _pal = (bool)o;
                _palToggle.IsChecked = _pal;
            }
        }

        protected virtual void OnEncodingChanged()
        {
            EncodingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateFormat()
        {
            Rgb[] pal = _pal ? Apple2Palettes.American : Apple2Palettes.European;
            Apple2TvSet tv = _fill ? (Apple2TvSet)new Apple2FillTv(pal) : (Apple2TvSet)new Apple2SimpleTv(pal);
            _format = new Apple2HiResImageFormat(tv);
            OnEncodingChanged();
        }
    }
}
